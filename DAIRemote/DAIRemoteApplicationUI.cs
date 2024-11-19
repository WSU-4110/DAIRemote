using AudioManager;
using DisplayProfileManager;
using Microsoft.Win32;
using UDPServerManagerForm;

namespace DAIRemote;

public partial class DAIRemoteApplicationUI : Form
{
    private TrayIconManager trayIconManager;
    private AudioOutputForm audioForm;
    private Panel audioFormPanel;
    private Form profileDialog;
    private ListBox profileListBox;
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

        this.Icon = new Icon("Resources/DAIRemoteLogo.ico");
        trayIconManager = new TrayIconManager(this);
        this.FormClosing += DAIRemoteApplicationUI_FormClosing;
        this.StartPosition = FormStartPosition.CenterScreen;

        SetStartupStatus();   // Checks onStartup default value to set
        InitializeDisplayProfilesLayouts(); // Initialize load and delete display profile flow layouts
        InitializeDisplayProfilesList();    // Initialize the form & listbox used for showing display profiles list
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        Task.Run(() =>
        {
            this.audioManager = AudioManager.AudioDeviceManager.GetInstance();

            this.Invoke((MethodInvoker)(() => InitializeAudioDropDown()));
        });
    }

    private void InitializeDisplayProfilesLayouts()
    {
        DisplayLoadProfilesLayout.Controls.Clear();
        DisplayDeleteProfilesLayout.Controls.Clear();

        foreach (string profile in DisplayConfig.GetDisplayProfiles())
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
        InitializeDisplayProfilesLayouts();
    }

    private void LoadProfileButton_Click(object sender, EventArgs e)
    {
        Button clickedButton = sender as Button;
        string profileName = clickedButton.Tag.ToString();
        DisplayConfig.SetDisplaySettings(profileName);
        InitializeDisplayProfilesLayouts();
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
            Location = new System.Drawing.Point(12, 460),
            Size = new System.Drawing.Size(760, 370),
        };

        audioForm = AudioOutputForm.GetInstance(audioManager);

        this.Controls.Add(this.audioFormPanel);
        audioFormPanel.Controls.Add(audioForm);
        audioForm.Show();
    }

    private void BtnAddDisplayConfig_Click(object sender, EventArgs e)
    {
        TrayIconManager.SaveNewProfile(DisplayConfig.GetDisplayProfilesDirectory());
        DisplayLoadProfilesLayout.Controls.Clear();
        DisplayDeleteProfilesLayout.Controls.Clear();
        InitializeDisplayProfilesLayouts();
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
        audioManager.CycleAudioDevice();
    }

    private void BtnSetAudioHotkey_Click(object sender, EventArgs e)
    {
        trayIconManager.GetHotkeyManager().ShowHotkeyInput("Audio Cycling", audioManager.CycleAudioDevice);
    }

    private void InitializeDisplayProfilesList()
    {
        profileDialog = new()
        {
            Text = "Display Profiles",
            Size = new Size(400, 300),
            StartPosition = FormStartPosition.CenterScreen
        };

        profileListBox = new()
        {
            Dock = DockStyle.Fill
        };
        profileDialog.Controls.Add(profileListBox);

        Button actionButton = new()
        {
            Text = "Select Profile",
            Dock = DockStyle.Bottom,
            Height = 50,
            FlatStyle = FlatStyle.Flat,
        };

        actionButton.Click += (s, e) =>
        {
            if (profileListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a profile.");
                return;
            }
            profileDialog.Close();
        };
        profileDialog.Controls.Add(actionButton);
    }

    private string ShowDisplayProfilesList(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        profileListBox.Items.Clear();
        profileListBox.Items.AddRange(DisplayConfig.GetDisplayProfiles().Select(profile => Path.GetFileNameWithoutExtension(profile)).ToArray());

        profileDialog.ShowDialog();
        return profileListBox.SelectedItem?.ToString();
    }

    private void BtnSetDisplayProfileHotkey_click(object sender, EventArgs e)
    {
        string profile = ShowDisplayProfilesList(DisplayConfig.GetDisplayProfilesDirectory());
        if (!string.IsNullOrEmpty(profile))
        {
            trayIconManager.GetHotkeyManager().ShowHotkeyInput(profile, () => DisplayConfig.SetDisplaySettings(profile));
        }
    }
}
