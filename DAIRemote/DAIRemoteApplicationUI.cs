using AudioDeviceManager;
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
        private AudioDeviceManager.AudioDeviceManager audioManager;
        private Label lblCurrentHotkey;
        private Keys selectedHotkey;

        public DAIRemoteApplicationUI()
        {
            UDPServerHost udpServer = new UDPServerHost();
            Thread udpThread = new Thread(() => udpServer.HostUDPServer())
            {
                IsBackground = true
            };
            udpThread.Start();
            selectedHotkey = Keys.None;
            InitializeHotkeySelection();

            InitializeComponent();
            InitializeDisplayProfilesLayouts();

            Task.Run(() =>
            {
                this.audioManager = AudioDeviceManager.AudioDeviceManager.GetInstance();

                this.Invoke((MethodInvoker)(() =>
                {
                    InitializeAudioComponents();
                }));
            });

            // Updated the property of the form itself to start with the color
            //this.BackColor = Color.FromArgb(50, 50, 50);
            this.Icon = new Icon("Resources/DAIRemoteLogo.ico");
            trayIconManager = new TrayIconManager(this);
            this.Load += DAIRemoteApplicationUI_Load;
            this.FormClosing += DAIRemoteApplicationUI_FormClosing;
            this.StartPosition = FormStartPosition.CenterScreen;

            LoadStartupSetting();   // Checks onStartup default value to set
        }
        private void InitializeHotkeySelection()
        {
            ComboBox hotkeyComboBox = new ComboBox();
            hotkeyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            hotkeyComboBox.Items.AddRange(Enum.GetNames(typeof(Keys))); // Add all keys
            hotkeyComboBox.SelectedIndexChanged += (s, e) =>
            {
                selectedHotkey = (Keys)Enum.Parse(typeof(Keys), hotkeyComboBox.SelectedItem.ToString());
                lblCurrentHotkey.Text = $"Current Hotkey: {selectedHotkey}";
            };

            this.Controls.Add(hotkeyComboBox); // Add it to the form
            hotkeyComboBox.Location = new Point(20, 150); // Position it on the form
            hotkeyComboBox.Width = 200;

            // Initialize the label to display the current hotkey
            lblCurrentHotkey = new Label();
            lblCurrentHotkey.Location = new Point(20, 180); // Position it on the form
            lblCurrentHotkey.AutoSize = true; // Adjust size based on content
            lblCurrentHotkey.ForeColor = Color.White; // Set text color
            lblCurrentHotkey.Text = "Current Hotkey: None"; // Default text

            this.Controls.Add(lblCurrentHotkey);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Check if the pressed key matches the selected hotkey
            if (keyData == selectedHotkey)
            {
                // Logic to cycle through audio devices
                audioManager.CycleToNextAudioDevice();
                return true; // Indicate the key was processed
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        private void DAIRemoteApplicationUI_Load(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void InitializeDisplayProfilesLayouts()
        {
            string[] displayProfiles = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles"), "*.json");

            foreach (var profile in displayProfiles)
            {
                Button loadProfileButton = new Button
                {
                    Text = Path.GetFileNameWithoutExtension(profile),
                    Width = 150,
                    Height = 50,
                    Margin = new Padding(10),
                    Tag = profile,
                    ForeColor = Color.White
                };

                Button deleteProfileButton = new Button
                {
                    Text = Path.GetFileNameWithoutExtension(profile),
                    Width = 150,
                    Height = 50,
                    Margin = new Padding(10),
                    Tag = profile,
                    ForeColor = Color.White
                };

                loadProfileButton.Click += loadProfileButton_Click;
                DisplayLoadProfilesLayout.Controls.Add(loadProfileButton);

                deleteProfileButton.Click += deleteProfileButton_Click;
                DisplayDeleteProfilesLayout.Controls.Add(deleteProfileButton);
            }
        }

        private void deleteProfileButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            string profileName = clickedButton.Tag.ToString();
            DisplayConfig.DeleteDisplaySettings(profileName);
        }

        private void loadProfileButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            string profileName = clickedButton.Tag.ToString();
            DisplayConfig.SetDisplaySettings(profileName);
        }

        private void DAIRemoteApplicationUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = false;
                trayIconManager.HideIcon();
            }
        }

        private void InitializeAudioComponents()
        {
            this.audioFormPanel = new Panel
            {
                Location = new System.Drawing.Point(10, 60),
                Size = new System.Drawing.Size(760, 370),
            };

            audioForm = AudioOutputForm.GetInstance(audioManager);

            this.Controls.Add(this.audioFormPanel);
            audioFormPanel.Controls.Add(audioForm);
            audioForm.Show();
        }

        public static void BtnAddDisplayConfig_Click(object sender, EventArgs e)
        {
            TrayIconManager.SaveNewProfile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles"));
        }

        private void BtnLoadDisplayConfig_Click(object sender, EventArgs e)
        {
            DisplayConfig.SetDisplaySettings("displayConfig" + ".json");
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

        private void BtnCycleAudioOutputs_Click(object sender, EventArgs e)
        {
            audioManager.CycleToNextAudioDevice();
        }
    }
}
