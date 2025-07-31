using DisplayProfileManager;
using System.Reflection;

namespace DAIRemote;

public class TrayIconManager
{
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;
    private readonly Form form;
    private readonly HotkeyManager hotkeyManager;
    private readonly AudioManager.AudioDeviceManager audioManager;
    public bool minimized = true;
    private Assembly assembly = Assembly.GetExecutingAssembly();

    private readonly Image aboutIcon;
    private readonly Image deleteProfileIcon;
    private readonly Image exitIcon;
    private readonly Image monitorIcon;
    private readonly Image saveProfileIcon;
    private readonly Image addProfileIcon;
    private readonly Image setHotkeyIcon;
    private readonly Image audioCyclingIcon;
    private readonly Image audioIcon;

    public TrayIconManager(Form form)
    {
        this.form = form;

        aboutIcon = Properties.Resources.About.ToBitmap();
        deleteProfileIcon = Properties.Resources.DeleteProfile.ToBitmap();
        exitIcon = Properties.Resources.Exit.ToBitmap();
        monitorIcon = Properties.Resources.Monitor;
        saveProfileIcon = Properties.Resources.SaveProfile.ToBitmap();
        addProfileIcon = Properties.Resources.AddProfile.ToBitmap();
        setHotkeyIcon = Properties.Resources.MonitorSetHotkey.ToBitmap();
        audioCyclingIcon = Properties.Resources.AudioCycling.ToBitmap();
        audioIcon = Properties.Resources.Audio.ToBitmap();

        audioManager = AudioManager.AudioDeviceManager.GetInstance();
        // Registers any prexisting hotkeys, otherwise initializes
        // an empty dictionary in preparation for hotkeys.
        hotkeyManager = new HotkeyManager();
        hotkeyManager.InitializeHotkeys();

        InitializeTrayIcon();
        DisplayConfig.NotificationRequested += OnNotificationRequested;
    }

    public HotkeyManager GetHotkeyManager()
    {
        return this.hotkeyManager;
    }

    private void InitializeTrayIcon()
    {
        trayMenu = CreateTrayMenu();
        trayIcon = new NotifyIcon
        {
            Text = "DAIRemote",
            Icon = Properties.Resources.DAIRemoteLogoIcon,
            ContextMenuStrip = trayMenu,
            Visible = true
        };

        // Listen for display profile changes
        DisplayProfileWatcher.Initialize(DisplayConfig.GetDisplayProfilesDirectory());
        DisplayProfileWatcher.ProfileChanged += OnProfilesChanged;

        // Listen to AudioDeviceManager's event handler for changes
        audioManager.AudioDevicesUpdated += OnAudioDevicesChanged;

        trayIcon.DoubleClick += (s, e) => ShowForm();
    }

    private ContextMenuStrip CreateTrayMenu()
    {
        ContextMenuStrip menu = new()
        {
            ForeColor = Color.Black,
            ShowImageMargin = true,
            Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
            AutoSize = true,
        };

        PopulateTrayMenu(menu);

        return menu;
    }

    public void RefreshSystemTray()
    {
        _ = form.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
        {
            hotkeyManager.InitializeHotkeys();
            PopulateTrayMenu(trayMenu);
        });
    }

    private void OnProfilesChanged(object sender, FileSystemEventArgs e)
    {
        _ = form.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
        {
            PopulateTrayMenu(trayMenu);
        });
    }

    private void OnAudioDevicesChanged(List<string> devices)
    {
        _ = form.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
        {
            PopulateTrayMenu(trayMenu);
        });
    }

    private void PopulateTrayMenu(ContextMenuStrip menu)
    {
        menu.Items.Clear();

        // Create Labels for loading, deleting, saving, and setting hotkeys
        // system tray submenus
        ToolStripLabel loadProfilesLabel = new("Loaded Profiles")
        {
            Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
            ForeColor = Color.Gray,
            Enabled = false,
        };

        ToolStripMenuItem deleteProfilesLabel = new("Select Profile to Delete")
        {
            Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
            ForeColor = Color.Gray,
            Enabled = false,
        };

        ToolStripMenuItem saveProfilesLabel = new("Overwrite Profile")
        {
            Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
            ForeColor = Color.Gray,
            Enabled = false,
        };

        ToolStripMenuItem setHotkeysLabel = new("Set Hotkeys")
        {
            Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
            ForeColor = Color.Gray,
            Enabled = false,
        };

        // Add item for handling the addition of new profiles through
        // the system tray save submenu
        ToolStripMenuItem addNewProfile = new("Add New Profile", addProfileIcon, (sender, e) => SaveNewProfile(DisplayConfig.GetDisplayProfilesDirectory()));

        // Create the save, delete, sethotkey submenus
        ToolStripMenuItem saveProfileMenuItem = new("Save Profile", saveProfileIcon);
        ToolStripMenuItem deleteProfileMenuItem = new("Delete Profile", deleteProfileIcon);
        ToolStripMenuItem setHotkeysMenuItem = new("Set Hotkeys", setHotkeyIcon);

        // Check if a hotkey exists for Audio Cycling and name accordingly
        // Also sets the handling of setting Audio Cycling hotkey
        string hotkeyText = hotkeyManager.hotkeyConfigs.ContainsKey("Audio Cycling")
            ? $" ({hotkeyManager.GetHotkeyText(hotkeyManager.hotkeyConfigs["Audio Cycling"])})"
            : "";
        ToolStripMenuItem audioCyclingHotkey = new($"Audio Cycling{hotkeyText}", audioCyclingIcon, (sender, e) =>
        {
            hotkeyManager.ShowHotkeyInput("Audio Cycling", () => audioManager.CycleAudioDevice());

            // Refresh to show the updated hotkey
            string updatedHotkeyText = hotkeyManager.hotkeyConfigs.ContainsKey("Audio Cycling")
                ? $" ({hotkeyManager.GetHotkeyText(hotkeyManager.hotkeyConfigs["Audio Cycling"])})"
                : "";

            // Find the existing menu item
            foreach (var item in setHotkeysMenuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem menuItem && menuItem.Text.StartsWith("Audio Cycling"))
                {
                    menuItem.Text = $"Audio Cycling{updatedHotkeyText}";
                    return;
                }
            }
        });

        // Add audio cycling item to the hotkey submenu
        _ = setHotkeysMenuItem.DropDownItems.Add(audioCyclingHotkey);

        // Add audio devices to hotkey submenu
        List<string> audioDevices = audioManager.ActiveDeviceNames;
        foreach (string audioDevice in audioDevices)
        {
            ToolStripMenuItem audioItem = new(audioDevice, audioIcon, (sender, e) =>
            {
                hotkeyManager.ShowHotkeyInput(audioDevice, () => audioManager.SetDefaultAudioDevice(audioDevice));

                // Refresh to show the updated hotkey
                string updatedHotkeyText = hotkeyManager.hotkeyConfigs.ContainsKey(audioDevice)
                    ? $" ({hotkeyManager.GetHotkeyText(hotkeyManager.hotkeyConfigs[audioDevice])})"
                    : "";

                // Find the existing menu item
                foreach (var item in setHotkeysMenuItem.DropDownItems)
                {
                    if (item is ToolStripMenuItem menuItem && menuItem.Text.StartsWith(audioDevice))
                    {
                        menuItem.Text = $"{audioDevice}{updatedHotkeyText}";
                        return;
                    }
                }
            });
            _ = setHotkeysMenuItem.DropDownItems.Add(audioItem);
        }

        // Separate audio hotkeys from display hotkeys in hotkey submenu
        _ = setHotkeysMenuItem.DropDownItems.Add(new ToolStripSeparator());

        // Add load profiles label to the main menu and separate it
        _ = menu.Items.Add(loadProfilesLabel);
        _ = menu.Items.Add(new ToolStripSeparator());

        // For each existing profile, add it as an item and modify it according
        // to each load, save, delete, and sethotkey submenu
        foreach (string profile in DisplayConfig.GetDisplayProfiles())
        {
            string fileName = Path.GetFileNameWithoutExtension(profile);
            ToolStripMenuItem profileLoadItem = new(fileName, monitorIcon, (sender, e) => DisplayConfig.SetDisplaySettings(profile));
            _ = menu.Items.Add(profileLoadItem);

            ToolStripMenuItem profileDeleteItem = new(fileName, monitorIcon, (sender, e) => DeleteProfile(profile));

            ToolStripMenuItem profileSaveItem = new(fileName, monitorIcon, (sender, e) => SaveProfile(profile));

            // Setting up hotkey funtionality for each profile
            hotkeyText = hotkeyManager.hotkeyConfigs.ContainsKey(fileName)
                ? $" ({hotkeyManager.GetHotkeyText(hotkeyManager.hotkeyConfigs[fileName])})"
                : "";
            ToolStripMenuItem profileSetHotkey = new($"{fileName}{hotkeyText}", monitorIcon, (sender, e) =>
            {
                hotkeyManager.ShowHotkeyInput(fileName, () => DisplayConfig.SetDisplaySettings(profile));

                // Refresh to show the updated hotkey
                string updatedHotkeyText = hotkeyManager.hotkeyConfigs.ContainsKey(fileName)
                    ? $" ({hotkeyManager.GetHotkeyText(hotkeyManager.hotkeyConfigs[fileName])})"
                    : "";

                // Find the existing menu item
                foreach (var item in setHotkeysMenuItem.DropDownItems)
                {
                    if (item is ToolStripMenuItem menuItem && menuItem.Text.StartsWith(fileName))
                    {
                        menuItem.Text = $"{fileName}{updatedHotkeyText}";
                        return;
                    }
                }
            });

            _ = deleteProfileMenuItem.DropDownItems.Add(profileDeleteItem);
            _ = saveProfileMenuItem.DropDownItems.Add(profileSaveItem);
            _ = setHotkeysMenuItem.DropDownItems.Add(profileSetHotkey);
        }

        // Separate loadable display profiles from the main menu
        // Then proceed to add the delete label, add new profile for save submenu
        // overwrite existing display profiles label in the save submenu,
        // and the set hotkeys label in the sethotkey submenu
        _ = menu.Items.Add(new ToolStripSeparator());
        deleteProfileMenuItem.DropDownItems.Insert(0, deleteProfilesLabel);
        deleteProfileMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());
        saveProfileMenuItem.DropDownItems.Insert(0, addNewProfile);
        saveProfileMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());
        saveProfileMenuItem.DropDownItems.Insert(2, saveProfilesLabel);
        saveProfileMenuItem.DropDownItems.Insert(3, new ToolStripSeparator());
        setHotkeysMenuItem.DropDownItems.Insert(0, setHotkeysLabel);
        setHotkeysMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());

        // Add the save, delete, and sethotkey submenus to the main system tray
        _ = menu.Items.Add(saveProfileMenuItem);
        _ = menu.Items.Add(deleteProfileMenuItem);
        _ = menu.Items.Add(setHotkeysMenuItem);

        // Allow the user to refresh audio devices
        // Helpful for when a new device is added
        // That was not present during application initialization
        ToolStripMenuItem refreshAudioDevices = new("Refresh Audio Devices", audioIcon, RefreshAudioDevices);
        // Create the icons for making monitors sleep, the about section, and exiting the application
        ToolStripMenuItem aboutMenuItem = new("About", aboutIcon, OnAboutClick);
        ToolStripMenuItem exitMenuItem = new("Exit", exitIcon, OnExit);

        // Separate sleeping monitors, and add the sleep, about, and exit to the main system tray menu
        _ = menu.Items.Add(new ToolStripSeparator());
        _ = menu.Items.Add(refreshAudioDevices);
        _ = menu.Items.Add(new ToolStripSeparator());
        _ = menu.Items.Add(aboutMenuItem);
        _ = menu.Items.Add(exitMenuItem);
    }

    public void ShowInputDialog(
    string title,
    string promptText,
    string inputHint,
    Action<string> onOkAction)
    {
        Form inputForm = new()
        {
            Width = 400,
            Height = 150,
            Text = title,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterScreen,
            MinimizeBox = false,
            MaximizeBox = false
        };

        Label promptLabel = new()
        {
            Left = 20,
            Top = 20,
            Text = promptText,
            AutoSize = true
        };

        TextBox inputBox = new()
        {
            Left = 20,
            Top = 50,
            Width = 350,
            PlaceholderText = inputHint
        };

        Button okButton = new()
        {
            Text = "OK",
            Left = 190,
            Width = 80,
            Top = 80,
            DialogResult = DialogResult.OK
        };
        okButton.Click += (sender, e) =>
        {
            string userInput = inputBox.Text;

            if (string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("Input cannot be empty.");
                return;
            }

            onOkAction?.Invoke(userInput); // Execute the provided function
            inputForm.Close();
        };

        Button cancelButton = new()
        {
            Text = "Cancel",
            Left = 290,
            Width = 80,
            Top = 80,
            DialogResult = DialogResult.Cancel
        };
        cancelButton.Click += (sender, e) => inputForm.Close();

        inputForm.Controls.Add(promptLabel);
        inputForm.Controls.Add(inputBox);
        inputForm.Controls.Add(okButton);
        inputForm.Controls.Add(cancelButton);

        inputForm.AcceptButton = okButton;
        inputForm.CancelButton = cancelButton;

        _ = inputForm.ShowDialog();
    }

    public void SaveNewProfile(string profilesFolderPath)
    {
        ShowInputDialog(
            "Save New Profile",
            "Please enter the profile name:",
            "New profile name here",
            userInput => DisplayConfig.SaveDisplaySettings(Path.Combine(profilesFolderPath, $"{userInput}.json")));
    }


    private void SaveProfile(string profilePath)
    {
        _ = DisplayConfig.SaveDisplaySettings(profilePath);
    }

    private void DeleteProfile(string profilePath)
    {
        _ = DisplayConfig.DeleteDisplaySettings(profilePath);
    }
    private void RefreshAudioDevices(object? sender, EventArgs e)
    {
        audioManager.RefreshAudioDeviceSubscriptions();
    }

    private void OnAboutClick(object? sender, EventArgs e)
    {
        string aboutMessage =
                      "DAIRemote is a versatile display, audio, and input remote for Windows desktops. It allows users to:\n\n" +
                      "• Save and load display profiles\n" +
                      "• Cycle through audio playback devices\n" +
                      "• Use an Android phone as a keyboard and mouse input\n\n" +
                      "All of these features can be controlled remotely, providing convenience from wherever you're sitting.";

        _ = MessageBox.Show(aboutMessage, "About DAIRemote", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void HideIcon()
    {
        trayIcon.Visible = false;
    }

    public void ShowIcon()
    {
        trayIcon.Visible = true;
    }

    public void OnShow(object sender, EventArgs e)
    {
        ShowForm();
    }

    private void ShowForm()
    {
        minimized = !minimized;
        if(!minimized)
        {
            form.Show();
            form.WindowState = FormWindowState.Normal;
            form.Activate();
        } else
        {
            form.Hide();
        }
    }

    private void OnExit(object sender, EventArgs e)
    {
        HideIcon();
        GetHotkeyManager().UnregisterHotkeys();
        Application.Exit();
    }

    private void OnNotificationRequested(object sender, NotificationEventArgs e)
    {
        SendNotification(trayIcon, e.NotificationText);
    }

    public void SendNotification(NotifyIcon trayIcon, string notificationText)
    {
        trayIcon.BalloonTipTitle = "DAIRemote";
        trayIcon.BalloonTipText = notificationText;
        trayIcon.BalloonTipIcon = ToolTipIcon.Error;
        trayIcon.ShowBalloonTip(3000);
    }
}
