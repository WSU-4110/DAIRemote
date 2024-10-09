using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPServerManager
{
    public class UDPServerHost
    {
        private bool isClientConnected = false;
        private UdpClient udpServer;
        private IPEndPoint remoteEP;

        public UDPServerHost()
        {
            udpServer = new UdpClient(11000);
            remoteEP = new IPEndPoint(IPAddress.Any, 11000);
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

                Debug.WriteLine($"Received handshake: {handshakeMessage}");

                // Check if the received message is the handshake request
                if (handshakeMessage.StartsWith("Connection requested"))
                {
                    // Send an approval message back to the client
                    string approvalMessage = "Approved";
                    byte[] approvalBytes = Encoding.ASCII.GetBytes(approvalMessage);
                    udpServer.Send(approvalBytes, approvalBytes.Length, remoteEP);

                    Debug.WriteLine("Sent approval back to client");
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
                if (InitiateHandshake())
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
                        } else if (receivedData.Equals("DroidHeartBeat", StringComparison.OrdinalIgnoreCase))
                        {
                            // Update last heartbeat time
                            lastHeartbeatTime = DateTime.Now;
                            Debug.WriteLine("Received heartbeat");

                            // Send heart beat acknowledgment to client
                            string shutdownAck = "HeartBeat Ack";
                            byte[] shutdownAckData = Encoding.ASCII.GetBytes(shutdownAck);
                            udpServer.Send(shutdownAckData, shutdownAckData.Length, remoteEP);
                        } else if (receivedData.StartsWith("Connection requested"))
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

        static void Main(string[] args)
        {

        }
    }
}
