using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace UDPServerManagerForm
{
    public class UDPServerHost
    {
        private bool isClientConnected = false;
        private UdpClient udpServer;
        private IPEndPoint remoteEP;

        // Store device names mapped to IP addresses
        private Dictionary<string, string> deviceHistory = new Dictionary<string, string>();

        public UDPServerHost()
        {
            string HistoryFilePath = GetFilePath("deviceHistory.json");

            LoadDeviceHistory();
            udpServer = new UdpClient(11000);
            remoteEP = new IPEndPoint(IPAddress.Any, 11000);
        }

        public void SaveDeviceHistory()
        {
            // Ensure the directory exists
            string HistoryFilePath = GetFilePath("deviceHistory.json");

            try
            {
                string json = JsonSerializer.Serialize(deviceHistory);
                File.WriteAllText(HistoryFilePath, json);
                Debug.WriteLine($"Device history saved to {HistoryFilePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving device history: {ex.Message}");
            }
        }

        public void LoadDeviceHistory()
        {
            string historyFilePath = GetFilePath("deviceHistory.json");

            try
            {
                string json = File.ReadAllText(historyFilePath);
                // Deserialize JSON to Dictionary
                deviceHistory = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                    ?? new Dictionary<string, string>();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON error: {ex.Message}");
                // Handle JSON parsing errors if needed
            }
            catch (FileNotFoundException)
            {
                // This should not occur because of EnsureFileExists
                Debug.WriteLine("Device history file not found. This should not happen.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading device history: {ex.Message}");
            }
        }

        // Method to add or update a device in the history
        private void ModifyDeviceHistory(string ipAddress, string deviceName)
        {
            if (!deviceHistory.ContainsKey(ipAddress))
            {
                deviceHistory.Add(ipAddress, deviceName);
            }
            else
            {
                deviceHistory[ipAddress] = deviceName;
            }
        }

        // Method to retrieve the device name from history, if it exists
        private string GetDeviceName(string ipAddress)
        {
            return deviceHistory.ContainsKey(ipAddress) ? deviceHistory[ipAddress] : "Unknown Device";
        }

        private string ExtractDeviceName(string handshakeMessage)
        {
            int byIndex = handshakeMessage.IndexOf("by ");
            if (byIndex >= 0)
            {
                // Extract everything after "by " as the device name
                return handshakeMessage.Substring(byIndex + 3).Trim();
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
                // Create an empty JSON file with an empty object for a dictionary
                File.WriteAllText(filePath, "{}");
            }

            return filePath;
        }

        // Method to handle the initial handshake
        public bool InitiateHandshake()
        {
            try
            {
                // Ensure udpServer is initialized before checking for data
                if (udpServer == null)
                {
                    Debug.WriteLine("Error: udpServer is not initialized.");
                    return false;
                }

                byte[] handshakeData = udpServer.Receive(ref remoteEP);
                string handshakeMessage = Encoding.ASCII.GetString(handshakeData);

                Debug.WriteLine($"Received handshake from {remoteEP.Address}:{remoteEP.Port}: {handshakeMessage}");

                // Check if the received message is the handshake request
                if (handshakeMessage.StartsWith("Connection requested"))
                {
                    string waitMessage = "Wait";
                    byte[] waitBytes = Encoding.ASCII.GetBytes(waitMessage);
                    udpServer.Send(waitBytes, waitBytes.Length, remoteEP);

                    UDPServerManagerForm form = new UDPServerManagerForm();
                    DialogResult connect = MessageBox.Show($"Allow ({remoteEP.Address}:{remoteEP.Port}) to connect?",
                        "Pending Connection", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    return form.HandleConnectionResult(connect, udpServer, remoteEP);
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

        // Method to handle the initial handshake
        public bool clientSearch()
        {
            try
            {
                if (udpServer == null)
                {
                    Debug.WriteLine("Error: udpServer is not initialized.");
                    return false;
                }

                byte[] handshakeData = udpServer.Receive(ref remoteEP);
                string handshakeMessage = Encoding.ASCII.GetString(handshakeData);

                if (handshakeMessage.StartsWith("Hello, I'm"))
                {
                    Debug.WriteLine($"Received client broadcast from {remoteEP.Address}:{remoteEP.Port}: {handshakeMessage}");
                    // Send an approval message back to the client
                    string approvalMessage = "Hello, I'm " + Environment.MachineName;
                    byte[] approvalBytes = Encoding.ASCII.GetBytes(approvalMessage);
                    udpServer.Send(approvalBytes, approvalBytes.Length, remoteEP);

                    Debug.WriteLine($"Sent reply to client's broadcast at {remoteEP.Address}:{remoteEP.Port}");
                    return true;
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

        // Method to start checking for handshake attempts in the background
        public async Task CheckForHandshake()
        {
            while (!isClientConnected)
            {
                if (clientSearch())
                {
                    Debug.WriteLine("Awaiting handshake...");
                }
                else if (InitiateHandshake())
                {
                    isClientConnected = true;
                    Debug.WriteLine("Handshake successful, starting message loop...");
                    break;
                }
            }
        }

        // Main message loop once the handshake is successful
        public void MessageLoop()
        {
            try
            {
                udpServer.Client.ReceiveTimeout = 15000; // 15-second timeout for receive

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
                        string receivedData = Encoding.ASCII.GetString(data);
                        Debug.WriteLine($"Received: {receivedData}");

                        // Check for shutdown message
                        if (receivedData.Equals("Shutdown requested", StringComparison.OrdinalIgnoreCase))
                        {
                            Debug.WriteLine("Shutdown message received. Exiting message loop...");

                            // Send shutdown acknowledgment to client
                            string shutdownAck = "Server shutting down";
                            byte[] shutdownAckData = Encoding.ASCII.GetBytes(shutdownAck);
                            udpServer.Send(shutdownAckData, shutdownAckData.Length, remoteEP);

                            isClientConnected = false;
                            break;
                        }
                        else if (receivedData.Equals("DroidHeartBeat", StringComparison.OrdinalIgnoreCase))
                        {
                            // Update last heartbeat time
                            lastHeartbeatTime = DateTime.Now;
                            Debug.WriteLine("Received heartbeat");

                            // Send heart beat acknowledgment to client
                            string shutdownAck = "HeartBeat Ack";
                            byte[] shutdownAckData = Encoding.ASCII.GetBytes(shutdownAck);
                            udpServer.Send(shutdownAckData, shutdownAckData.Length, remoteEP);
                        }
                        else if (receivedData.StartsWith("Connection requested"))
                        {
                            // Send an approval message back to the client
                            string approvalMessage = "Approved";
                            byte[] approvalBytes = Encoding.ASCII.GetBytes(approvalMessage);
                            udpServer.Send(approvalBytes, approvalBytes.Length, remoteEP);

                            Debug.WriteLine("Sent approval back to client");
                        }
                    }
                    catch (SocketException e)
                    {
                        if (e.SocketErrorCode == SocketError.TimedOut)
                        {
                            // Timeout is expected, continue loop to wait for a message or heartbeat
                            Debug.WriteLine("waiting for next message...");
                            continue;
                        }
                        else
                        {
                            // Other socket errors should be logged and handled
                            Debug.WriteLine("Error in message loop: " + e.Message);
                            break; // Exit on unexpected errors
                        }
                    }
                }
            }
            catch (ObjectDisposedException e)
            {
                Debug.WriteLine("Error: UdpClient has been disposed: " + e.Message);
            }
        }

        // Method to start the UDP server
        public async Task hostUDPServer()
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
                    udpServer.Close();
                    udpServer = null;
                }

                // Reinitialize the UDP server for new connections
                udpServer = new UdpClient(11000);
                remoteEP = new IPEndPoint(IPAddress.Any, 11000);
            }
        }
    }
}
