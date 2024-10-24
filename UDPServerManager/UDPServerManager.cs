﻿using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using UDPServerManager;

namespace UDPServerManagerForm
{
    public class UDPServerHost
    {
        private bool isClientConnected = false;
        private UdpClient udpServer;
        private IPEndPoint remoteEP;
        private string clientAddress;
        private readonly int serverPort = 11000;

        public UDPServerHost()
        {
            udpServer = new UdpClient(serverPort);
            remoteEP = new IPEndPoint(IPAddress.Any, serverPort);
        }

        public class DeviceHistoryEntry
        {
            public string DeviceName { get; set; }
            public string IpAddress { get; set; }
            public DateTime Timestamp { get; set; }
        }

        public void SaveDeviceHistory(string ipAddress, string deviceName = "")
        {
            // Ensure the directory exists
            string filePath = GetFilePath("deviceHistory.json");

            DeviceHistoryEntry ipData = new()
            {
                DeviceName = deviceName,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now
            };

            // Read the existing JSON file if it exists
            List<DeviceHistoryEntry> ipList = [];
            if (File.Exists(filePath))
            {
                string existingData = File.ReadAllText(filePath);
                ipList = JsonSerializer.Deserialize<List<DeviceHistoryEntry>>(existingData);
            }

            // Add the new IP to the list
            ipList.Add(ipData);

            // Write the updated list to the JSON file
            string jsonData = JsonSerializer.Serialize(ipList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);

        }

        public bool SearchDeviceHistory(string ipAddress)
        {
            string historyFilePath = GetFilePath("deviceHistory.json");

            string existingData = File.ReadAllText(historyFilePath);
            List<DeviceHistoryEntry> ipList = JsonSerializer.Deserialize<List<DeviceHistoryEntry>>(existingData);

            // Check if any entry in the list has the same IP address
            foreach (DeviceHistoryEntry entry in ipList)
            {
                if (entry.IpAddress == ipAddress)
                {
                    return true;
                }
            }

            return false;
        }

        private string ExtractDeviceName(string handshakeMessage)
        {
            int byIndex = handshakeMessage.IndexOf("by ");
            if (byIndex >= 0)
            {
                // Extract everything after "by " as the device name
                return handshakeMessage[(byIndex + 3)..].Trim();
            }
            return null;
        }

        private string GetFilePath(string fileName)
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, fileName);

            if (!File.Exists(filePath))
            {
                // Create an empty JSON file with an empty array
                File.WriteAllText(filePath, "[]");
            }

            return filePath;
        }

        // Method to handle the initial handshake
        public bool InitiateHandshake()
        {
            try
            {
                byte[] handshakeData = udpServer.Receive(ref remoteEP);
                string handshakeMessage = Encoding.ASCII.GetString(handshakeData);

                Debug.WriteLine($"Received handshake from {remoteEP.Address}:{remoteEP.Port}: {handshakeMessage}");

                // Check if the received message is the handshake request
                if (handshakeMessage.StartsWith("Connection requested"))
                {
                    return AwaitApproval(ExtractDeviceName(handshakeMessage));
                }
                else if (handshakeMessage.StartsWith("Hello, I'm"))
                {
                    Debug.WriteLine($"Received client broadcast from {remoteEP.Address}:{remoteEP.Port}: {handshakeMessage}");
                    // Send an approval message back to the client
                    SendUdpMessage("Hello, I'm " + Environment.MachineName);

                    Debug.WriteLine($"Sent reply to client's broadcast at {remoteEP.Address}:{remoteEP.Port}\nAwaiting handshake...");
                    return false;
                }
                return false;
            }
            catch (SocketException e)
            {
                Debug.WriteLine("Error during handshake: " + e.Message);
                return false;
            }
            catch (ObjectDisposedException e)
            {
                Debug.WriteLine("Error: UdpClient has been disposed: " + e.Message);
                return false;
            }
        }

        public bool AwaitApproval(string deviceName)
        {
            Debug.WriteLine("Checking for approval...");
            SendUdpMessage("Wait");
            string ip = remoteEP.Address.ToString();
            if (SearchDeviceHistory(ip))
            {
                string approvalMessage = "Approved";
                byte[] approvalBytes = Encoding.ASCII.GetBytes(approvalMessage);
                udpServer.Send(approvalBytes, approvalBytes.Length, remoteEP);
                clientAddress = ip;

                Debug.WriteLine("Approval granted, prior history");
                return true;
            }
            else
            {
                UDPServerManagerForm form = new();
                DialogResult connect = MessageBox.Show($"Allow ({remoteEP.Address}:{remoteEP.Port}) to connect?",
                    "Pending Connection", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (form.HandleConnectionResult(connect, udpServer, remoteEP))
                {
                    SaveDeviceHistory(ip, deviceName);
                    clientAddress = ip;
                    Debug.WriteLine("Approval granted by user");
                    return true;
                }
            }
            return false;
        }

        // Method to start checking for handshake attempts in the background
        public async Task CheckForHandshake()
        {
            while (!isClientConnected)
            {
                try
                {
                    bool handshakeSuccessful = await Task.Run(InitiateHandshake);

                    if (handshakeSuccessful)
                    {
                        isClientConnected = true;
                        Debug.WriteLine("Handshake successful, starting message loop...");
                        MessageLoop();
                        break;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error during handshake: " + e.Message);
                }
            }
        }

        public bool IsClient(string ipAddress)
        {
            return clientAddress != null && clientAddress.Equals(ipAddress);
        }

        // Main message loop once the handshake is successful
        public void MessageLoop()
        {
            try
            {
                udpServer.Client.ReceiveTimeout = 20000; // 20-second timeout for receive

                DateTime lastHeartbeatTime = DateTime.Now;
                TimeSpan heartbeatTimeout = TimeSpan.FromSeconds(60);

                while (isClientConnected)
                {
                    try
                    {
                        // Check if the heartbeat timeout has been exceeded
                        if (DateTime.Now - lastHeartbeatTime > heartbeatTimeout)
                        {
                            Debug.WriteLine("No heartbeat received in 60 seconds. Disconnecting...");
                            isClientConnected = false; // Exit the loop
                            break;
                        }

                        // Blocking call: wait for a message from the client
                        byte[] data = udpServer.Receive(ref remoteEP);

                        // Check if input is by client
                        if (IsClient(remoteEP.Address.ToString()))
                        {
                            string receivedData = Encoding.ASCII.GetString(data);
                            Debug.WriteLine($"Received: {receivedData}");

                            // Handle received data
                            HandleReceivedData(receivedData, ref lastHeartbeatTime);
                        }

                    }
                    catch (SocketException e)
                    {
                        if (e.SocketErrorCode == SocketError.TimedOut)
                        {
                            Debug.WriteLine("waiting for next message...");
                            continue;
                        }
                        else
                        {
                            Debug.WriteLine("Error in message loop: " + e.Message);
                            break;
                        }
                    }
                }
            }
            catch (ObjectDisposedException e)
            {
                Debug.WriteLine("Error: UdpClient has been disposed: " + e.Message);
            }
            finally
            {
                // Ensure UDP client is properly disposed
                if (udpServer != null)
                {
                    udpServer.Close();
                    udpServer = null;
                }
            }
        }

        private void HandleReceivedData(string receivedData, ref DateTime lastHeartbeatTime)
        {
            if (receivedData.Equals("Shutdown requested", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("Shutdown message received. Exiting message loop...");
                isClientConnected = false;
            }
            else if (receivedData.Equals("DroidHeartBeat", StringComparison.OrdinalIgnoreCase))
            {
                lastHeartbeatTime = DateTime.Now;
                Debug.WriteLine("Acknowledged heartbeat");

                SendUdpMessage("HeartBeat Ack");
            }
            else if (receivedData.StartsWith("Hello, I'm"))
            {
                SendUdpMessage("Hello, I'm " + Environment.MachineName);
                Debug.WriteLine("Sent response to broadcast");
                InitiateHandshake();
            }
            else if (receivedData.StartsWith("Connection requested"))
            {
                AwaitApproval(ExtractDeviceName(receivedData));
            }
            else
            {
                RetrieveCommand(receivedData);
            }
        }

        private void SendUdpMessage(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            udpServer.Send(data, data.Length, remoteEP);
        }

        public void RetrieveCommand(string command)
        {
            string[] parts = command.Split([' '], 2);
            string action = parts[0];

            Debug.WriteLine(action);
            switch (action)
            {
                case "MOUSE_MOVE":
                    string[] moveParts = parts[1].Split(' ');
                    var currentPos = MouseOperations.GetCursorPosition();

                    if (moveParts.Length >= 2)
                    {
                        float x = float.Parse(moveParts[0]);
                        float y = float.Parse(moveParts[1]);

                        MouseOperations.SetCursorPosition((int)(x + currentPos.X), (int)(y + currentPos.Y));
                    }
                    else
                    {
                        Debug.WriteLine("MOUSE_MOVE command received but not enough parameters.");
                    }
                    break;
                case "MOUSE_LMB":
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                    Thread.Sleep(25);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                    break;
                case "KEYBOARD_WRITE":
                    string key = parts[1];
                    if (key.Length > 1)
                    {
                        // Special keys such as {F1}, {ENTER}, etc.
                        SendKeys.SendWait(key);
                    }
                    else
                    {
                        key = Regex.Replace(key, "[+^%~(){}]", "{$0}");
                        SendKeys.SendWait(key);
                    }
                    break;
                case "MOUSE_LMB_HOLD":
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                    Thread.Sleep(25);
                    break;
                case "MOUSE_RMB":
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                    Thread.Sleep(25);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                    break;
                case "MOUSE_SCROLL":
                    int scrollAmount = (int)float.Parse(parts[1]);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.Wheel, scrollAmount);
                    break;
                default:
                    Console.WriteLine("Unknown command: " + command);
                    break;
            }
        }

        // Method to start the UDP server
        public async Task HostUDPServer()
        {
            while (true)
            {
                Debug.WriteLine("UDP Server is listening...");

                // Start checking for handshake attempts in the background
                await CheckForHandshake(); // Wait until the handshake succeeds

                // Enter the message loop once the handshake is successful
                if (isClientConnected)
                {
                    MessageLoop();
                }

                // Reinitialize the UDP server for new connections
                udpServer = new UdpClient(serverPort);
                remoteEP = new IPEndPoint(IPAddress.Any, serverPort);
            }
        }
    }
}
