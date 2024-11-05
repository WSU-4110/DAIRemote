﻿using DisplayProfileManager;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using UDPServerManager;

namespace UDPServerManagerForm;

public class UDPServerHost
{
    private bool isClientConnected = false;
    private UdpClient udpServer;
    private IPEndPoint remoteEP;
    private string clientAddress;
    private readonly int serverPort = 11000;
    private DateTime lastHeartbeatTime;
    private TimeSpan heartbeatTimeout;
    private AudioManager.AudioDeviceManager audioManager;

    public UDPServerHost()
    {
        udpServer = new UdpClient(serverPort);
        remoteEP = new IPEndPoint(IPAddress.Any, serverPort);
    }

    private void SetLastHeartbeat(DateTime time)
    {
        this.lastHeartbeatTime = time;
    }

    private DateTime GetLastHeartbeat() { return lastHeartbeatTime; }

    private void SetHeartbeatTimeout(TimeSpan time)
    {
        this.heartbeatTimeout = time;
    }

    private TimeSpan GetHeartbeatTimeout() { return heartbeatTimeout; }

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

    // Method to handle the initial handshake
    public bool InitiateHandshake()
    {
        try
        {
            byte[] handshakeData = udpServer.Receive(ref remoteEP);
            string handshakeMessage = Encoding.ASCII.GetString(handshakeData);

            Debug.WriteLine($"Received handshake from {remoteEP.Address}:{remoteEP.Port}: {handshakeMessage}");

            return HandleReceivedData(handshakeMessage);
        }
        catch (SocketException e)
        {
            Debug.WriteLine("Error during handshake: " + e.Message);
        }
        catch (ObjectDisposedException e)
        {
            Debug.WriteLine("Error: UdpClient has been disposed: " + e.Message);
        }
        return false;
    }

    public bool AwaitApproval(string deviceName)
    {
        DeviceHistoryManager deviceHistoryManager = new();
        Debug.WriteLine("Checking for approval...");
        SendUdpMessage("Wait");
        string ip = remoteEP.Address.ToString();
        if (deviceHistoryManager.SearchDeviceHistory(ip))
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
                deviceHistoryManager.SaveDeviceHistory(ip, deviceName);
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
                    // Ensure we have an instance of audioManager in case of input
                    // regarding volume
                    await Task.Run(() => this.audioManager = AudioManager.AudioDeviceManager.GetInstance());

                    isClientConnected = true;
                    Debug.WriteLine("Handshake successful, starting message loop...");
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
            SetLastHeartbeat(DateTime.Now);
            SetHeartbeatTimeout(TimeSpan.FromSeconds(2700));

            while (isClientConnected)
            {
                try
                {
                    // Blocking, wait for a message from the client
                    byte[] data = udpServer.Receive(ref remoteEP);
                    string receivedData = Encoding.ASCII.GetString(data);
                    HandleReceivedData(receivedData);

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

    private bool HandleReceivedData(string receivedData)
    {
        if (receivedData.StartsWith("Hello, I'm"))
        {
            SendUdpMessage("Hello, I'm " + Environment.MachineName);
            Debug.WriteLine($"Sent reply to client's broadcast at {remoteEP.Address}:{remoteEP.Port}\nAwaiting handshake...");
        }
        else if (receivedData.StartsWith("Hello, DAI"))
        {
            SendUdpMessage("Hello, I'm " + Environment.MachineName);
            Debug.WriteLine("Sent response to broadcast search");
        }
        else if (receivedData.StartsWith("Connection requested"))
        {
            return AwaitApproval(ExtractDeviceName(receivedData));
        }

        if (isClientConnected)
        {
            // Check if the heartbeat timeout has been exceeded
            if (DateTime.Now - GetLastHeartbeat() > GetHeartbeatTimeout())
            {
                Debug.WriteLine("No heartbeat received in 45 minutes. Disconnecting...");
                isClientConnected = false;
            }
            if (IsClient(remoteEP.Address.ToString()))
            {
                if (receivedData.Equals("Shutdown requested", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("Shutdown message received. Exiting message loop...");
                    isClientConnected = false;
                }
                else if (receivedData.Equals("DroidHeartBeat", StringComparison.OrdinalIgnoreCase))
                {
                    SetLastHeartbeat(DateTime.Now);
                    SendUdpMessage("HeartBeat Ack");
                    Debug.WriteLine("Acknowledged heartbeat");
                }
                else
                {
                    RetrieveCommand(receivedData);
                }
            }
        }
        return false;
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
                MouseManager.MousePoint currentPos = MouseManager.GetCursorPosition();

                if (moveParts.Length >= 2)
                {
                    float x = float.Parse(moveParts[0]);
                    float y = float.Parse(moveParts[1]);

                    MouseManager.SetCursorPosition((int)Math.Floor(x + currentPos.X), (int)Math.Floor(y + currentPos.Y));
                }
                else
                {
                    Debug.WriteLine("MOUSE_MOVE command received but not enough parameters.");
                }
                break;
            case "MOUSE_LMB":
                MouseManager.MouseEvent(MouseManager.MouseEventFlags.LeftDown);
                Thread.Sleep(25);
                MouseManager.MouseEvent(MouseManager.MouseEventFlags.LeftUp);
                break;
            case "KEYBOARD_WRITE":
                string key = parts[1];
                if (key.Length > 1)
                {
                    // Special keys such as {F1}, {ENTER}, etc.
                    if (key.StartsWith("WIN"))
                    {
                        if (key.Equals("WIN()"))
                        {
                            SpecialKeys.PressKey(SpecialKeys.LWin);
                        }
                        else
                        {
                            int from = key.IndexOf("WIN(") + "WIN(".Length;
                            int to = key.LastIndexOf(")");
                            string winKeys = key.Substring(from, to - from);
                            if (winKeys.Length > 1)
                            {
                                SpecialKeys.KeyDown(SpecialKeys.LWin);
                                SendKeys.SendWait(winKeys);
                                SpecialKeys.KeyUp(SpecialKeys.LWin);
                            }
                            else
                            {
                                winKeys = Regex.Replace(winKeys, "[+^%~(){}]", "{$0}");
                                SpecialKeys.KeyDown(SpecialKeys.LWin);
                                SendKeys.SendWait(winKeys);
                                SpecialKeys.KeyUp(SpecialKeys.LWin);
                            }
                        }
                    }
                    else
                    {
                        SendKeys.SendWait(key);
                    }
                }
                else
                {
                    key = Regex.Replace(key, "[+^%~(){}]", "{$0}");
                    SendKeys.SendWait(key);
                }
                break;
            case "MOUSE_LMB_HOLD":
                MouseManager.MouseEvent(MouseManager.MouseEventFlags.LeftDown);
                Thread.Sleep(25);
                break;
            case "MOUSE_RMB":
                MouseManager.MouseEvent(MouseManager.MouseEventFlags.RightDown);
                Thread.Sleep(25);
                MouseManager.MouseEvent(MouseManager.MouseEventFlags.RightUp);
                break;
            case "MOUSE_SCROLL":
                int scrollAmount = (int)float.Parse(parts[1]);
                MouseManager.MouseEvent(MouseManager.MouseEventFlags.Wheel, scrollAmount);
                break;
            case "MOUSE_MMB":
                MouseManager.MouseEvent(MouseManager.MouseEventFlags.MiddleDown);
                Thread.Sleep(25);
                MouseManager.MouseEvent(MouseManager.MouseEventFlags.MiddleUp);
                break;
            case "AUDIO":
                if (parts[1] == "UP")
                {
                    audioManager.IncreaseVolume(5);
                }
                else if (parts[1] == "DOWN")
                {
                    audioManager.DecreaseVolume(5);
                }
                else if (parts[1] == "MUTE")
                {
                    audioManager.ToggleAudioMute();
                }
                else if (parts[1] == "Devices")
                {
                    List<string> devices = audioManager.ActiveDeviceNames;
                    string list = string.Join(",", devices);

                    SendUdpMessage("AudioDevices: " + list + "|Volume: " + audioManager.GetVolume() + "|DefaultAudioDevice: " + audioManager.GetDefaultAudioDevice().FullName);
                }
                else if (parts[1] == "TogglePlay")
                {
                    SpecialKeys.PressKey(SpecialKeys.playPauseTrack);
                }
                else if (parts[1] == "PreviousTrack")
                {
                    SpecialKeys.PressKey(SpecialKeys.previousTrack);
                }
                else if (parts[1] == "NextTrack")
                {
                    SpecialKeys.PressKey(SpecialKeys.nextTrack);
                }
                break;
            case "AudioVolume":
                audioManager.SetVolume(Convert.ToDouble(parts[1]));
                break;
            case "AudioConnect":
                audioManager.SetDefaultAudioDevice(parts[1]);
                break;
            case "DISPLAY":
                try
                {
                    // Get all .json files from the directory (display profiles)
                    string[] displayProfiles = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles"), "*.json");
                    string displayProfileName = string.Join(",", Array.ConvertAll(displayProfiles, Path.GetFileNameWithoutExtension));

                    Debug.WriteLine(displayProfileName);
                    SendUdpMessage("DisplayProfiles: " + displayProfileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error returning display profiles to client: " + ex.Message);
                }
                break;
            case "DisplayConnect":
                DisplayConfig.SetDisplaySettings(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles/" + parts[1] + ".json"));
                break;
            case "HOST":
                SendUdpMessage("HostName: " + Environment.MachineName);
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
