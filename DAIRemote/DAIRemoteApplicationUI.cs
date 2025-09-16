using DisplayProfileManager;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UDPServerManagerForm;

namespace DAIRemote;

public partial class DAIRemoteApplicationUI : Form
{
    private readonly TrayIconManager trayIconManager;
    private Form profileDialog;
    private Form audioDialog;
    private ListBox profileListBox;
    private ListBox audioListBox;
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

        this.Icon = Properties.Resources.DAIRemoteLogoIcon;
        trayIconManager = new TrayIconManager(this);

        this.WindowState = FormWindowState.Minimized;
        this.FormClosing += DAIRemoteApplicationUI_FormClosing;
        this.StartPosition = FormStartPosition.CenterScreen;

        SetStartupStatus();   // Checks onStartup default value to set
        InitializeDisplayProfilesLayouts(); // Initialize load and delete display profile flow layouts
        InitializeDisplayProfilesList();    // Initialize the form & listbox used for showing display profiles list
        InitializeAudioDevicesList();

        // Listen for display profile changes
        DisplayProfileWatcher.Initialize(DisplayConfig.GetDisplayProfilesDirectory());
        DisplayProfileWatcher.ProfileChanged += OnProfilesChanged;

        // Hide the form initially
        this.Hide();
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

        foreach (string profile in DisplayConfig.GetDisplayProfiles())
        {
            // Panel to represent a profile tile
            Panel profileTile = new()
            {
                Width = 100,
                Height = 100,
                BackgroundImage = Properties.Resources.Monitor,
                BackgroundImageLayout = ImageLayout.Center,
                Tag = profile
            };

            // Setting default click action for the panel (Load Profile)
            profileTile.Click += LoadProfileButton_Click;

            // Border color using a Paint event
            profileTile.Paint += (sender, e) => ControlPaint.DrawBorder(
                    e.Graphics,
                    profileTile.ClientRectangle,
                    Color.LightSlateGray,  // Border color
                    ButtonBorderStyle.Solid);

            // Label for the profile name
            Label profileNameLabel = new()
            {
                Text = Path.GetFileNameWithoutExtension(profile),
                AutoSize = false,
                TextAlign = ContentAlignment.TopCenter,
                BackColor = Color.Transparent,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold),
                Width = 90,
                Height = 85,
                Top = 15,
                Left = (100 - 90) / 2, // Center horizontally within the panel
                Tag = profile
            };
            profileNameLabel.Click += LoadProfileButton_Click;

            // Create a button for additional options
            Button optionsButton = new()
            {
                Text = "⋮",
                Width = 22,
                Height = 22,
                BackColor = Color.DarkGray,
                ForeColor = Color.White,
                Tag = profile,
                Top = 78,
                Left = 78
            };

            // Add options menu on button click
            optionsButton.Click += (sender, e) => ShowProfileOptionsMenu(sender);

            // Add components to the profile tile
            profileTile.Controls.Add(optionsButton);
            profileTile.Controls.Add(profileNameLabel);

            // Add the tile to the layout
            DisplayLoadProfilesLayout.Controls.Add(profileTile);
        }
    }

    private void ShowProfileOptionsMenu(object sender)
    {
        Button optionsButton = sender as Button;
        ContextMenuStrip optionsMenu = new();

        // Add Rename option
        ToolStripMenuItem renameItem = new("Rename");
        renameItem.Click += (s, e) => RenameProfileButton_Click(optionsButton, e);

        // Add Set Hotkey option
        ToolStripMenuItem setHotkeyItem = new("Set Hotkey");
        setHotkeyItem.Click += (s, e) => SetHotkeyProfileButton_Click(optionsButton, e);

        // Add Set Default Audio Device option
        ToolStripMenuItem setDefaultAudioDevice = new("Set Audio");
        setDefaultAudioDevice.Click += (s, e) => SetDefaultAudioDevice_Click(optionsButton, e);

        // Add Save option
        ToolStripMenuItem saveItem = new("Overwrite");
        saveItem.Click += (s, e) => SaveProfileButton_Click(optionsButton, e);

        // Add Delete option
        ToolStripMenuItem deleteItem = new("Delete");
        deleteItem.Click += (s, e) => DeleteProfileButton_Click(optionsButton, e);

        // Add options to the context menu
        _ = optionsMenu.Items.Add(renameItem);
        _ = optionsMenu.Items.Add(setHotkeyItem);
        _ = optionsMenu.Items.Add(setDefaultAudioDevice);
        _ = optionsMenu.Items.Add(saveItem);
        _ = optionsMenu.Items.Add(deleteItem);

        // Show the menu at the button's location
        optionsMenu.Show(optionsButton, new Point(0, optionsButton.Height));
    }

    private void LoadProfileButton_Click(object sender, EventArgs e)
    {
        // Tag from either a Panel or Label
        var control = sender as Control;
        string profilePath = control?.Tag?.ToString();

        if (!string.IsNullOrEmpty(profilePath))
        {
            _ = DisplayConfig.SetDisplaySettings(profilePath);
        }
    }

    private void RenameProfileButton_Click(object sender, EventArgs e)
    {
        string profilePath = ((sender as Button)?.Tag ?? (sender as Panel)?.Tag).ToString();

        trayIconManager.ShowInputDialog(
            "Rename Profile",
            "Please enter the new profile name:",
            "New name for profile here",
            userInput =>
            {
                _ = DisplayConfig.RenameDisplayProfile(profilePath, userInput);
            }
        );
    }

    // Set hotkey tooltip function
    private void SetHotkeyProfileButton_Click(object sender, EventArgs e)
    {
        string profilePath = ((sender as Button)?.Tag ?? (sender as Panel)?.Tag).ToString();
        trayIconManager.GetHotkeyManager().ShowHotkeyInput(Path.GetFileNameWithoutExtension(profilePath), () => DisplayConfig.SetDisplaySettings(profilePath));
        trayIconManager.RefreshSystemTray();
    }

    // Set hotkey main application pop up function to allow choosing a profile from a list and setting the hotkey.
    private void BtnSetDisplayProfileHotkey_click(object sender, EventArgs e)
    {
        string fileName = ShowDisplayProfilesList(DisplayConfig.GetDisplayProfilesDirectory());
        if (!string.IsNullOrEmpty(fileName))
        {
            trayIconManager.GetHotkeyManager().ShowHotkeyInput(fileName, () => DisplayConfig.SetDisplaySettings(DisplayConfig.GetFullDisplayProfilePath(fileName)));
            trayIconManager.RefreshSystemTray();
        }
    }

    private void SetDefaultAudioDevice_Click(object sender, EventArgs e)
    {
        ShowAudioDevicesList(((sender as Button)?.Tag ?? (sender as Panel)?.Tag).ToString());
    }

    private void SaveProfileButton_Click(object sender, EventArgs e)
    {
        string profilePath = ((sender as Button)?.Tag ?? (sender as Panel)?.Tag).ToString();
        _ = DisplayConfig.SaveDisplaySettings(profilePath);
    }

    private void DeleteProfileButton_Click(object sender, EventArgs e)
    {
        string profilePath = ((sender as Button)?.Tag ?? (sender as Panel)?.Tag).ToString();
        _ = DisplayConfig.DeleteDisplaySettings(profilePath);
    }

    private void DAIRemoteApplicationUI_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            this.Hide();
            trayIconManager.minimized = true;
            e.Cancel = true;
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
        trayIconManager.SaveNewProfile(DisplayConfig.GetDisplayProfilesDirectory());
        DisplayLoadProfilesLayout.Controls.Clear();
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
        trayIconManager.RefreshSystemTray();
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

    private void InitializeAudioDevicesList()
    {
        audioDialog = new()
        {
            Text = "Audio Devices",
            Size = new Size(400, 300),
            StartPosition = FormStartPosition.CenterScreen
        };

        audioListBox = new()
        {
            Dock = DockStyle.Fill
        };
        audioDialog.Controls.Add(audioListBox);

        System.Windows.Forms.Button actionButton = new()
        {
            Text = "Select Device",
            Dock = DockStyle.Bottom,
            Height = 50,
            FlatStyle = FlatStyle.Flat,
        };

        actionButton.Click += (s, e) =>
        {
            if (audioListBox.SelectedItem == null)
            {
                _ = MessageBox.Show("Please select a new default audio device.");
                return;
            }
            audioDialog.DialogResult = DialogResult.OK;
            audioDialog.Close();
        };
        audioDialog.Controls.Add(actionButton);
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

    private void ShowAudioDevicesList(string profilePath)
    {
        // Read current settings first
        string json = File.ReadAllText(profilePath);
        JObject displaySettings = JObject.Parse(json);
        string currentDefaultDevice = displaySettings["defaultAudioDevice"]?.ToString();

        // Populate the list
        audioListBox.Items.Clear();
        audioListBox.Items.AddRange(audioManager.ActiveDeviceNames.Cast<object>().ToArray());

        // Set the current default device as selected if it exists in the list
        if (!string.IsNullOrEmpty(currentDefaultDevice))
        {
            int index = audioListBox.Items.Cast<string>().ToList().IndexOf(currentDefaultDevice);
            if (index >= 0)
            {
                audioListBox.SelectedIndex = index;
            }
        }

        // Show dialog and check result
        if (audioDialog.ShowDialog() == DialogResult.OK && audioListBox.SelectedItem != null)
        {
            displaySettings["defaultAudioDevice"] = audioListBox.SelectedItem.ToString();
            File.WriteAllText(profilePath, displaySettings.ToString(Formatting.Indented));
        }
    }

    private void DAIRemoteApplicationUI_Resize(object sender, EventArgs e)
    {
        if (FormWindowState.Minimized == this.WindowState)
        {
            this.Hide();
            trayIconManager.minimized = true;
        }
    }
}