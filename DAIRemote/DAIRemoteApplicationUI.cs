using AudioManager;
using DisplayProfileManager;
using Microsoft.Win32;
using UDPServerManagerForm;
using System.Text.Json;



namespace DAIRemote
{
    public partial class DAIRemoteApplicationUI : Form
    {
        private TrayIconManager trayIconManager;
        private AudioOutputForm audioForm;
        private Panel audioFormPanel;
        private AudioDeviceManager.AudioDeviceManager audioManager;
        private HotkeyManager hotkeyManager;
namespace DAIRemote;

public partial class DAIRemoteApplicationUI : Form
{
    private TrayIconManager trayIconManager;
    private AudioOutputForm audioForm;
    private Panel audioFormPanel;
    private AudioManager.AudioDeviceManager audioManager;

    public DAIRemoteApplicationUI()
    {
        UDPServerHost udpServer = new();
        Thread udpThread = new(() => udpServer.HostUDPServer())
        {
            IsBackground = true
        };
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
            this.Invoke((MethodInvoker)(() => InitializeAudioDropDown()));
        });

            this.Icon = new Icon("Resources/DAIRemoteLogo.ico");
            trayIconManager = new TrayIconManager(this);
            this.Load += DAIRemoteApplicationUI_Load;
            this.FormClosing += DAIRemoteApplicationUI_FormClosing;
            this.StartPosition = FormStartPosition.CenterScreen;
        // Updated the property of the form itself to start with the color
        //this.BackColor = Color.FromArgb(50, 50, 50);
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

            hotkeyManager = new HotkeyManager(lblCurrentHotkey, audioManager);
            LoadSavedHotkeys(); // Only load hotkeys once

            hotkeyComboBox.SelectedIndexChanged += (s, e) =>
            {
                var selectedHotkey = (Keys)Enum.Parse(typeof(Keys), hotkeyComboBox.SelectedItem.ToString());
                hotkeyManager.SetHotkey(selectedHotkey);
            };
        }

        private void LoadSavedHotkeys()
        {
            var savedHotkeys = hotkeyManager.LoadHotkeys();

            hotkeyComboBox.Items.Clear();
            foreach (var hotkey in savedHotkeys)
            {
                hotkeyComboBox.Items.Add(hotkey);
            }
            hotkeyComboBox.Items.Add("None"); // Add "None" as a selectable option
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            hotkeyManager.HandleKeyPress(keyData);
            return base.ProcessCmdKey(ref msg, keyData);
        }
       
        private void btnSetHotkey_Click(object sender, EventArgs e)
        {
            if (hotkeyComboBox.SelectedItem != null)
            {
                var selectedHotkey = (Keys)Enum.Parse(typeof(Keys), hotkeyComboBox.SelectedItem.ToString());
                hotkeyManager.SetHotkey(selectedHotkey);
            }
        }

        SetStartupStatus();   // Checks onStartup default value to set
    }

        private void DAIRemoteApplicationUI_Load(object sender, EventArgs e)
        {
            hotkeyManager = new HotkeyManager(lblCurrentHotkey, audioManager);
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

    private void DAIRemoteApplicationUI_Load(object sender, EventArgs e)
    {
        this.Hide();
    }

    private void InitializeDisplayProfilesLayouts()
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string[] displayProfilesDirectory = Directory.GetFiles(folderPath, "*.json");

        foreach (string profile in displayProfilesDirectory)
        {
            Button loadProfileButton = new()
            {
                Text = Path.GetFileNameWithoutExtension(profile),
                Width = 150,
                Height = 50,
                Margin = new Padding(10),
                Tag = profile,
                ForeColor = Color.White
            };

            Button deleteProfileButton = new()
            {
                Text = Path.GetFileNameWithoutExtension(profile),
                Width = 150,
                Height = 50,
                Margin = new Padding(10),
                Tag = profile,
                ForeColor = Color.White
            };

            loadProfileButton.Click += LoadProfileButton_Click;
            DisplayLoadProfilesLayout.Controls.Add(loadProfileButton);

            deleteProfileButton.Click += DeleteProfileButton_Click;
            DisplayDeleteProfilesLayout.Controls.Add(deleteProfileButton);
        }
    }

    private void DeleteProfileButton_Click(object sender, EventArgs e)
    {
        Button clickedButton = sender as Button;
        string profileName = clickedButton.Tag.ToString();
        DisplayConfig.DeleteDisplaySettings(profileName);
    }

    private void LoadProfileButton_Click(object sender, EventArgs e)
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

    private void InitializeAudioDropDown()
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
    private void BtnAddDisplayConfig_Click(object sender, EventArgs e)
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

    private static void AddToStartup(string startupKey, string appName, string appPath)
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

    private static bool IsAppInStartup()
    {
        using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
        return key.GetValue("DAIRemote") != null;
    }

    private void SetStartupStatus()
    {
        checkBoxStartup.Checked = IsAppInStartup();
    }

        private void BtnCycleAudioOutputs_Click(object sender, EventArgs e)
        {
            audioManager.CycleToNextAudioDevice();
        }
        private void hotkeyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hotkeyComboBox.SelectedItem != null)
            {
                var selectedHotkey = hotkeyComboBox.SelectedItem.ToString();

                if (selectedHotkey != "None")
                {
                    // Parse the selected item to a Keys enum value
                    if (Enum.TryParse<Keys>(selectedHotkey, out Keys parsedHotkey))
                    {
                        hotkeyManager.SetHotkey(parsedHotkey);
                    }
                }
                else
                {
                    hotkeyManager.SetHotkey(Keys.None); // Handle "None" selection
                }
            }
        }


        private void lblCurrentHotkey_Click(object sender, EventArgs e)
        {

        }
    private void BtnCycleAudioOutputs_Click(object sender, EventArgs e)
    {
        audioManager.CycleAudioDevice();
    }
}
