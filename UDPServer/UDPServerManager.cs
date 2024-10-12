using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDPServerManager
{
    public class UDPServerHost
    {
        public void hostUDPServer()
        {
            UdpClient udpServer = new UdpClient(11000);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 11000);

            try
            {
                Debug.WriteLine("UDP Server is listening...");

                while (true)
                {
                    // Receive data from any client
                    byte[] data = udpServer.Receive(ref remoteEP);
                    string receivedData = Encoding.ASCII.GetString(data);
                    Debug.WriteLine($"Received: {receivedData} from {remoteEP.Address}");

                    // Prepare the response with the server's IP address
                    string responseData = $"Hello, I'm the server at {udpServer.Client.LocalEndPoint}";
                    byte[] responseBytes = Encoding.ASCII.GetBytes(responseData);

                    // Send the response back to the client
                    udpServer.Send(responseBytes, responseBytes.Length, remoteEP);
                    Debug.WriteLine("Sent response back to client");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            finally
            {
                udpServer.Close();
            }
        }

        static void Main(string[] args)
        {
            UDPServerHost serverHost = new UDPServerHost();
            serverHost.hostUDPServer(); // Start the server
        }
    }
}
