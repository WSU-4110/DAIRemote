using System;
using System.Threading;
using System.Windows.Forms;
using DisplayProfileManager;

namespace DAIRemote
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private TrayIconManager trayIconManager;
        private Panel audioFormPanel;

        public Form1()
        {
            // Start the UDP server on a separate thread
            UDPServer udpServer = new UDPServer();
            Thread udpThread = new Thread(() => udpServer.hostUDPServer());
            udpThread.IsBackground = true;
            udpThread.Start();

            InitializeComponent();
            InitializeCustomComponents();

            this.Icon = new Icon("Resources/DAIRemoteLogo.ico");
            trayIconManager = new TrayIconManager(this);
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide(); 
        }

        private void BtnShowAudioOutputs_Click(object sender, EventArgs e)
        {
            audioFormPanel.Controls.Clear();

            AudioOutputForm audioForm = new AudioOutputForm
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill 
            };

            audioFormPanel.Controls.Add(audioForm); 
            audioForm.Show(); 
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = false; 
                trayIconManager.HideIcon();
            }
        }

        private void InitializeCustomComponents()
        {
            this.audioFormPanel = new Panel
            {
                Location = new System.Drawing.Point(10, 60), 
                Size = new System.Drawing.Size(760, 370), 
            };

            this.Controls.Add(this.audioFormPanel);
        }
       
        private void BtnSaveDisplayConfig_Click(object sender, EventArgs e)
        {
            string fileName = "displayConfig";
            DisplayConfig.SaveDisplaySettings(fileName + ".json");
        }
    }
}