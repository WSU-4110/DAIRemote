using AudioManager;
using DisplayProfileManager;
using Microsoft.Win32;
using UDPServerManagerForm;
using System.Text.Json;
using AudioManager;

namespace DAIRemote
{
    public partial class DAIRemoteApplicationUI : Form
    {
        private TrayIconManager trayIconManager;
        private AudioOutputForm audioForm;
        private Panel audioFormPanel;
        private AudioManager.AudioDeviceManager audioManager;
        private HotkeyManager hotkeyManager;

        public DAIRemoteApplicationUI()
        {
            UDPServerHost udpServer = new UDPServerHost();
            Thread udpThread = new Thread(() => udpServer.HostUDPServer()) { IsBackground = true };
            udpThread.Start();

            InitializeComponent();
            InitializeDisplayProfilesLayouts();

            Task.Run(() =>
            {
                this.audioManager = AudioManager.AudioDeviceManager.GetInstance();

                this.Invoke((MethodInvoker)(() =>
                {
                    InitializeAudioComponents();
                    InitializeHotkeySelection();
                }));
            });

            this.Icon = new Icon("Resources/DAIRemoteLogo.ico");
            trayIconManager = new TrayIconManager(this);
            this.Load += DAIRemoteApplicationUI_Load;
            this.FormClosing += DAIRemoteApplicationUI_FormClosing;
            this.StartPosition = FormStartPosition.CenterScreen;

            LoadStartupSetting();
        }

        private void InitializeHotkeySelection()
        {
            if (hotkeyComboBox.Items.Count == 0)
            {
                hotkeyComboBox.Items.AddRange(Enum.GetNames(typeof(Keys)));
            }

            hotkeyManager = new HotkeyManager(lblCurrentHotkey, audioManager, hotkeyComboBox);

            var savedHotkeys = hotkeyManager.LoadHotkeys();
            hotkeyComboBox.Items.Clear();
            hotkeyComboBox.Items.Add("None");
            foreach (var hotkey in savedHotkeys)
            {
                hotkeyComboBox.Items.Add(hotkey);
            }

            if (savedHotkeys.Count > 0)
            {
                hotkeyComboBox.SelectedItem = savedHotkeys[0];
                lblCurrentHotkey.Text = $"Current Hotkey: {savedHotkeys[0]}";
            }
            else
            {
                lblCurrentHotkey.Text = "Current Hotkey: None";
            }

            hotkeyComboBox.SelectedIndexChanged += hotkeyManager.OnComboBoxSelectionChanged;
        }

        private void LoadSavedHotkeys()
        {
            var savedHotkeys = hotkeyManager.LoadHotkeys();
            hotkeyComboBox.Items.Clear();
            foreach (var hotkey in savedHotkeys)
            {
                hotkeyComboBox.Items.Add(hotkey);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            hotkeyManager.HandleKeyPress(keyData);
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnSetHotkey_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Press the key you want to set as a hotkey.");
            KeySelectionForm keySelectionForm = new KeySelectionForm(hotkeyManager);
            keySelectionForm.ShowDialog();
        }

        private void DAIRemoteApplicationUI_Load(object sender, EventArgs e)
        {
            hotkeyManager = new HotkeyManager(lblCurrentHotkey, audioManager, hotkeyComboBox);
            var loadedHotkeys = hotkeyManager.LoadHotkeys();

            hotkeyComboBox.Items.Clear();
            foreach (var hotkey in loadedHotkeys)
            {
                hotkeyComboBox.Items.Add(hotkey);
            }

            hotkeyComboBox.Items.Add("None");

            if (loadedHotkeys.Count > 0)
            {
                var firstHotkey = loadedHotkeys[0];
                lblCurrentHotkey.Text = $"Current Hotkey: {firstHotkey}";
                hotkeyComboBox.SelectedItem = firstHotkey;
            }
            else
            {
                lblCurrentHotkey.Text = "Current Hotkey: None";
            }
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

        private void CheckBoxStartup_CheckedChanged(object sender, EventArgs e)
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
                RemoveFromStartup(startupKey, appName);
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

        private static void RemoveFromStartup(string startupKey, string appName)
        {
            try
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(startupKey, true);
                if (key.GetValue(appName) != null)
                {
                    key.DeleteValue(appName);
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
            audioManager.CycleAudioDevice();
        }

        private void hotkeyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hotkeyComboBox.SelectedItem != null)
            {
                var selectedHotkey = hotkeyComboBox.SelectedItem.ToString();

                if (selectedHotkey != "None")
                {
                    if (Enum.TryParse<Keys>(selectedHotkey, out Keys parsedHotkey))
                    {
                        hotkeyManager.SetHotkey(parsedHotkey);
                    }
                }
                else
                {
                    hotkeyManager.SetHotkey(Keys.None);
                }
            }
        }

        private void lblCurrentHotkey_Click(object sender, EventArgs e) { }
    }
}
