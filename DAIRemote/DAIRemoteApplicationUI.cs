using AudioManager;
using DisplayProfileManager;
using UDPServerManagerForm;

namespace DAIRemote
{
    public partial class DAIRemoteApplicationUI : Form
    {
        private TrayIconManager trayIconManager;
        private AudioOutputForm audioForm;
        private Panel audioFormPanel;

        public DAIRemoteApplicationUI()
        {
            UDPServerHost udpServer = new UDPServerHost();
            Thread udpThread = new Thread(() => udpServer.hostUDPServer());
            udpThread.IsBackground = true;
            udpThread.Start();

            InitializeComponent();
            InitializeCustomComponents();

            this.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.Icon = new Icon("Resources/DAIRemoteLogo.ico");
            trayIconManager = new TrayIconManager(this);
            this.Load += DAIRemoteApplicationUI_Load;
            this.FormClosing += DAIRemoteApplicationUI_FormClosing;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void DAIRemoteApplicationUI_Load(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void BtnShowAudioOutputs_Click(object sender, EventArgs e)
        {
            if (audioForm == null || audioForm.IsDisposed)
            {
                audioForm = new AudioOutputForm
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                audioFormPanel.Controls.Add(audioForm);
                audioForm.Show();
            }
            else
            {
                if (audioForm.Visible)
                {
                    audioForm.Hide();
                }
                else
                {
                    audioForm.Show();
                }
            }
        }

        private void DAIRemoteApplicationUI_FormClosing(object sender, FormClosingEventArgs e)
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
            string fileName = profileNameTextBox.Text;
            if (fileName != "")
            {
                DisplayConfig.SaveDisplaySettings(fileName + ".json");
            }
            else
            {
                MessageBox.Show("Invalid input, name cannot be empty");
            }

            profileNameTextBox.Clear();
        }

        private void BtnLoadDisplayConfig_Click(object sender, EventArgs e)
        {
            DisplayConfig.SetDisplaySettings("displayConfig" + ".json");
        }
    }
}
