using DisplayProfileManager;
using Microsoft.Win32;
using UDPServerManagerForm;

namespace DAIRemote;

public partial class DAIRemoteApplicationUI : Form
{
    private readonly TrayIconManager trayIconManager;
    private Form profileDialog;
    private ListBox profileListBox;
    private AudioManager.AudioDeviceManager audioManager;
    public static event EventHandler<NotificationEventArgs> NotificationRequested;

    public static void RequestNotification(string notificationText)
    {
        NotificationRequested?.Invoke(null, new NotificationEventArgs(notificationText));
    }

    public DAIRemoteApplicationUI()
    {
        UDPServerHost udpServer = new();
        Thread udpThread = new(() => udpServer.HostUDPServer())
        {
            IsBackground = true
        };
        udpThread.Start();

        InitializeComponent();

        this.Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DAIRemoteLogo.ico"));
        trayIconManager = new TrayIconManager(this);
        this.FormClosing += DAIRemoteApplicationUI_FormClosing;
        this.StartPosition = FormStartPosition.CenterScreen;

        SetStartupStatus();   // Checks onStartup default value to set
        InitializeDisplayProfilesLayouts(); // Initialize load and delete display profile flow layouts
        InitializeDisplayProfilesList();    // Initialize the form & listbox used for showing display profiles list

        // Listen for display profile changes
        DisplayProfileWatcher.Initialize(DisplayConfig.GetDisplayProfilesDirectory());
        DisplayProfileWatcher.ProfileChanged += OnProfilesChanged;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        _ = Task.Run(() => this.Invoke((MethodInvoker)(() => InitializeAudioDropDown())));
    }

    private void OnProfilesChanged(object sender, FileSystemEventArgs e)
    {
        _ = this.BeginInvoke((MethodInvoker)delegate
        {
            InitializeDisplayProfilesLayouts();
        });
    }

    private void InitializeDisplayProfilesLayouts()
    {
        DisplayLoadProfilesLayout.Controls.Clear();
        DisplayDeleteProfilesLayout.Controls.Clear();

        foreach (string profile in DisplayConfig.GetDisplayProfiles())
        {
            // Create buttons for loading and deleting profiles
            System.Windows.Forms.Button loadProfileButton = new()
            {
                Text = Path.GetFileNameWithoutExtension(profile),
                Width = 150,
                Height = 50,
                Margin = new Padding(10),
                Tag = profile,
                ForeColor = Color.White
            };

            System.Windows.Forms.Button deleteProfileButton = new()
            {
                Text = Path.GetFileNameWithoutExtension(profile),
                Width = 150,
                Height = 50,
                Margin = new Padding(10),
                Tag = profile,
                ForeColor = Color.White
            };

            // Set onClick actions
            loadProfileButton.Click += LoadProfileButton_Click;
            deleteProfileButton.Click += DeleteProfileButton_Click;

            // Add buttons to layout
            DisplayLoadProfilesLayout.Controls.Add(loadProfileButton);
            DisplayDeleteProfilesLayout.Controls.Add(deleteProfileButton);
        }
    }

    private void DeleteProfileButton_Click(object sender, EventArgs e)
    {
        _ = DisplayConfig.DeleteDisplaySettings(((System.Windows.Forms.Button)sender).Tag.ToString());
    }

    private void LoadProfileButton_Click(object sender, EventArgs e)
    {
        _ = DisplayConfig.SetDisplaySettings(((System.Windows.Forms.Button)sender).Tag.ToString());
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
        this.audioManager = AudioManager.AudioDeviceManager.GetInstance();

        AudioComboBox.SelectedIndexChanged += this.AudioComboBox_SelectedIndexChanged;
        this.Controls.Add(AudioComboBox);

        audioManager.AudioDevicesUpdated += OnAudioDevicesUpdated;
        PopulateAudioComboBox(audioManager.ActiveDeviceNames);
    }

    private void OnAudioDevicesUpdated(List<string> devices)
    {
        Invoke(new Action(() => PopulateAudioComboBox(audioManager.ActiveDeviceNames)));
    }

    private void PopulateAudioComboBox(List<string> audioDevices)
    {
        AudioComboBox.Items.Clear();
        string defaultAudioDevice = audioManager.GetDefaultAudioDevice().FullName;
        int defaultIndex = -1;

        for (int i = 0; i < audioDevices.Count; i++)
        {
            _ = AudioComboBox.Items.Add(audioDevices[i]);
            if (audioDevices[i] == defaultAudioDevice)
            {
                defaultIndex = i;
            }
        }

        if (defaultIndex != -1)
        {
            AudioComboBox.SelectedIndex = defaultIndex;
        }
    }

    private void AudioComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (AudioComboBox.SelectedItem is string selectedDevice && selectedDevice != audioManager.GetDefaultAudioDevice().FullName)
        {
            audioManager.SetDefaultAudioDevice(selectedDevice);
        }
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
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(startupKey, true);
            key.SetValue(appName, $"\"{appPath}\"");
        }
        catch (Exception)
        {
            RequestNotification("Error adding to startup");
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
        catch (Exception)
        {
            RequestNotification("Error removing from startup");
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

        System.Windows.Forms.Button actionButton = new()
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
                _ = MessageBox.Show("Please select a profile.");
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
            _ = Directory.CreateDirectory(folderPath);
        }

        profileListBox.Items.Clear();
        profileListBox.Items.AddRange(DisplayConfig.GetDisplayProfiles().Select(profile => Path.GetFileNameWithoutExtension(profile)).ToArray());

        _ = profileDialog.ShowDialog();
        return profileListBox.SelectedItem?.ToString();
    }

    private void BtnSetDisplayProfileHotkey_click(object sender, EventArgs e)
    {
        string profile = ShowDisplayProfilesList(DisplayConfig.GetDisplayProfilesDirectory());
        if (!string.IsNullOrEmpty(profile))
        {
            trayIconManager.GetHotkeyManager().ShowHotkeyInput(profile, () => DisplayConfig.SetDisplaySettings(profile));
            trayIconManager.RefreshSystemTray();
        }
    }
}
