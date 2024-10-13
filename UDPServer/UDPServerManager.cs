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
                    // Receive data from client
                    byte[] data = udpServer.Receive(ref remoteEP);
                    string receivedData = Encoding.ASCII.GetString(data);
                    Debug.WriteLine($"Received: {receivedData}");

                    /*// Send a response back to the client
                    string responseData = "Data received!";
                    byte[] responseBytes = Encoding.ASCII.GetBytes(responseData);
                    udpServer.Send(responseBytes, responseBytes.Length, remoteEP);
                    Debug.WriteLine("Sent response back to client");
                    */
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

        }
    }
}