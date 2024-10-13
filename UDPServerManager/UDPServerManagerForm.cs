using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDPServerManagerForm
{
    public partial class UDPServerManagerForm : Form
    {
        public UDPServerManagerForm()
        {
            InitializeComponent();
        }

        public bool HandleConnectionResult(DialogResult connect, UdpClient udpServer, IPEndPoint remoteEP)
        {
            if (connect == DialogResult.Yes)
            {
                string approvalMessage = "Approved";
                byte[] approvalBytes = Encoding.ASCII.GetBytes(approvalMessage);
                udpServer.Send(approvalBytes, approvalBytes.Length, remoteEP);
                Debug.WriteLine($"Sent approval back to client at {remoteEP.Address}:{remoteEP.Port}");
                return true;
            }
            else
            {
                string denialMessage = "Connection attempt declined.";
                byte[] approvalBytes = Encoding.ASCII.GetBytes(denialMessage);
                udpServer.Send(approvalBytes, approvalBytes.Length, remoteEP);
                Debug.WriteLine($"Declined client {remoteEP.Address}:{remoteEP.Port}");
                return false;
            }
        }
    }
}
