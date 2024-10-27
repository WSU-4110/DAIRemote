using AudioManager;
using DisplayProfileManager;
using Microsoft.Win32;
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
            InitializeDisplayLoadProfilesLayout();
            InitializeDisplayDeleteProfilesLayout();

            this.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.Icon = new Icon("Resources/DAIRemoteLogo.ico");
            trayIconManager = new TrayIconManager(this);
            this.Load += DAIRemoteApplicationUI_Load;
            this.FormClosing += DAIRemoteApplicationUI_FormClosing;
            this.StartPosition = FormStartPosition.CenterScreen;

            LoadStartupSetting();   // Checks onStartup default value to set
        }

        private void DAIRemoteApplicationUI_Load(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void InitializeDisplayLoadProfilesLayout()
        {
            string[] displayProfiles = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles"), "*.json");

            foreach (var profile in displayProfiles)
            {
                Button profileButton = new Button
                {
                    Text = Path.GetFileNameWithoutExtension(profile),
                    Width = 150,
                    Height = 50,
                    Margin = new Padding(10),
                    Tag = profile
                };

                profileButton.Click += ProfileButton_Click;
                DisplayLoadProfilesLayout.Controls.Add(profileButton);
            }
        }

        private void ProfileButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            string profileName = clickedButton.Tag.ToString();
            DisplayConfig.SetDisplaySettings(profileName);
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

        private void checkBoxStartup_CheckedChanged(object sender, EventArgs e)
        {
            string startupKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            string appName = "DAIRemote";
            string appPath = Application.ExecutablePath;

            if (checkBoxStartup.Checked)
            {
                AddToStartup(startupKey, appName, appPath);
            }
            else
            {
                RemoveFromStartup(startupKey, appName, appPath);
            }
        }

        private void AddToStartup(string startupKey, string appName, string appPath)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(startupKey, true))
                {
                    key.SetValue(appName, $"\"{appPath}\"");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding to startup: " + ex.Message);
            }
        }

        private void RemoveFromStartup(string startupKey, string appName, string appPath)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(startupKey, true))
                {
                    if (key.GetValue(appName) != null)
                    {
                        key.DeleteValue(appName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing from startup: " + ex.Message);
            }
        }

        private bool IsAppInStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
            {
                return key.GetValue("DAIRemote") != null;
            }
        }

        private void LoadStartupSetting()
        {
            checkBoxStartup.Checked = IsAppInStartup();
        }

        private void InitializeDisplayDeleteProfilesLayout()
        {
            string[] displayProfiles = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles"), "*.json");

            foreach (var profile in displayProfiles)
            {
                Button profileButton = new Button
                {
                    Text = Path.GetFileNameWithoutExtension(profile),
                    Width = 150,
                    Height = 50,
                    Margin = new Padding(10),
                    Tag = profile
                };

                profileButton.Click += ProfileButton_Click_2;
                DisplayDeleteProfilesLayout.Controls.Add(profileButton);
            }
        }

        private void ProfileButton_Click_2(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            string profileName = clickedButton.Tag.ToString();
            DisplayConfig.DeleteDisplaySettings(profileName);
        }
    }
}
