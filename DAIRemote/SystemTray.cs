using DisplayProfileManager;

namespace DAIRemote;

public class TrayIconManager
{
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;
    private Form form;
    private FileSystemWatcher displayProfileDirWatcher;
    private readonly string displayProfilesFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles");
    private HotkeyManager hotkeyManager;

    private Image aboutIcon;
    private Image DAIRemoteLogoIcon;
    private Image deleteProfileIcon;
    private Image exitIcon;
    private Image monitorIcon;
    private Image saveProfileIcon;
    private Image turnOffAllMonitorsIcon;
    private Image addProfileIcon;

    public TrayIconManager(Form form)
    {
        this.form = form;

        aboutIcon = Image.FromFile("Resources/About.ico");
        DAIRemoteLogoIcon = Image.FromFile("Resources/DAIRemoteLogo.ico");
        deleteProfileIcon = Image.FromFile("Resources/DeleteProfile.ico");
        exitIcon = Image.FromFile("Resources/Exit.ico");
        monitorIcon = Image.FromFile("Resources/Monitor.ico");
        saveProfileIcon = Image.FromFile("Resources/SaveProfile.ico");
        turnOffAllMonitorsIcon = Image.FromFile("Resources/TurnOffAllMonitors.ico");
        addProfileIcon = Image.FromFile("Resources/AddProfile.ico");

        if (!Directory.Exists(displayProfilesFolderPath))
        {
            Directory.CreateDirectory(displayProfilesFolderPath);
        }

        hotkeyManager = new HotkeyManager();
        hotkeyManager.LoadHotkeyConfigs();

        InitializeTrayIcon();
    }

    private void InitializeTrayIcon()
    {
        trayMenu = CreateTrayMenu();
        trayIcon = new NotifyIcon
        {
            Text = "DAIRemote",
            Icon = new Icon("Resources/DAIRemoteLogo.ico"),
            ContextMenuStrip = trayMenu,
            Visible = true
        };

        displayProfileDirWatcher = new FileSystemWatcher(displayProfilesFolderPath)
        {
            NotifyFilter = NotifyFilters.FileName
        };
        displayProfileDirWatcher.Created += OnProfilesChanged;
        displayProfileDirWatcher.Deleted += OnProfilesChanged;
        displayProfileDirWatcher.Renamed += OnProfilesChanged;
        displayProfileDirWatcher.EnableRaisingEvents = true;

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

    private void OnProfilesChanged(object sender, FileSystemEventArgs e)
    {
        if (form.InvokeRequired)
        {
            form.BeginInvoke((MethodInvoker)delegate
            {
                PopulateTrayMenu(trayMenu);
            });
        }
        else
        {
            PopulateTrayMenu(trayMenu);
        }
    }

    private void PopulateTrayMenu(ContextMenuStrip menu)
    {
        menu.Items.Clear();

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

        ToolStripMenuItem addNewProfile = new("Add New Profile", addProfileIcon, (sender, e) => SaveNewProfile(displayProfilesFolderPath));

        ToolStripMenuItem setHotkeysLabel = new("Set Hotkeys")
        {
            Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
            ForeColor = Color.Gray,
            Enabled = false,
        };

        ToolStripMenuItem saveProfileMenuItem = new("Save Profile", saveProfileIcon);
        ToolStripMenuItem deleteProfileMenuItem = new("Delete Profile", deleteProfileIcon);
        ToolStripMenuItem setHotkeysMenuItem = new("Set Hotkeys", monitorIcon);

        ToolStripMenuItem profileSetHotkey = null;

        string hotkeyText = hotkeyManager.hotkeyConfigs.ContainsKey("Audio Cycling")
            ? $" ({hotkeyManager.GetHotkeyText(hotkeyManager.hotkeyConfigs["Audio Cycling"])})"
            : "";
        ToolStripMenuItem audioCyclingHotkey = new($"Audio Cycling{hotkeyText}", monitorIcon, (sender, e) =>
        {
            hotkeyManager.ShowHotkeyInput("Audio Cycling", () => AudioManager.AudioDeviceManager.GetInstance().CycleAudioDevice());

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
        setHotkeysMenuItem.DropDownItems.Add(audioCyclingHotkey);
        setHotkeysMenuItem.DropDownItems.Add(new ToolStripSeparator());

        menu.Items.Add(loadProfilesLabel);
        menu.Items.Add(new ToolStripSeparator());

        string[] displayProfiles = Directory.GetFiles(displayProfilesFolderPath, "*.json");

        foreach (string profile in displayProfiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(profile);
            ToolStripMenuItem profileLoadItem = new(fileName, monitorIcon, (sender, e) => DisplayConfig.SetDisplaySettings(profile));
            menu.Items.Add(profileLoadItem);

            ToolStripMenuItem profileDeleteItem = new(fileName, monitorIcon, (sender, e) => DeleteProfile(profile));

            ToolStripMenuItem profileSaveItem = new(fileName, monitorIcon, (sender, e) => SaveProfile(profile));


            // Setting up hotkey funtionality for each profile
            hotkeyText = hotkeyManager.hotkeyConfigs.ContainsKey(fileName)
                ? $" ({hotkeyManager.GetHotkeyText(hotkeyManager.hotkeyConfigs[fileName])})"
                : "";
            profileSetHotkey = new ToolStripMenuItem($"{fileName}{hotkeyText}", monitorIcon, (sender, e) =>
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

            deleteProfileMenuItem.DropDownItems.Add(profileDeleteItem);
            saveProfileMenuItem.DropDownItems.Add(profileSaveItem);
            setHotkeysMenuItem.DropDownItems.Add(profileSetHotkey);
        }

        menu.Items.Add(new ToolStripSeparator());
        deleteProfileMenuItem.DropDownItems.Insert(0, deleteProfilesLabel);
        deleteProfileMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());
        saveProfileMenuItem.DropDownItems.Insert(0, addNewProfile);
        saveProfileMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());
        saveProfileMenuItem.DropDownItems.Insert(2, saveProfilesLabel);
        saveProfileMenuItem.DropDownItems.Insert(3, new ToolStripSeparator());
        setHotkeysMenuItem.DropDownItems.Insert(0, setHotkeysLabel);
        setHotkeysMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());
        menu.Items.Add(saveProfileMenuItem);
        menu.Items.Add(deleteProfileMenuItem);
        menu.Items.Add(setHotkeysMenuItem);

        ToolStripMenuItem turnOffAllMonitorsItem = new("Turn Off All Monitors", turnOffAllMonitorsIcon, TurnOffMonitors);
        ToolStripMenuItem aboutMenuItem = new("About", aboutIcon, OnAboutClick);
        ToolStripMenuItem exitMenuItem = new("Exit", exitIcon, OnExit);

        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(turnOffAllMonitorsItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(aboutMenuItem);
        menu.Items.Add(exitMenuItem);
    }

    public static void SaveNewProfile(string profilesFolderPath)
    {
        Form inputForm = new()
        {
            Width = 400,
            Height = 150,
            Text = "Save New Profile",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterScreen,
            MinimizeBox = false,
            MaximizeBox = false
        };

        Label promptLabel = new()
        {
            Left = 20,
            Top = 20,
            Text = "Please enter the profile name:",
            AutoSize = true
        };

        TextBox inputBox = new()
        {
            Left = 20,
            Top = 50,
            Width = 350
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

            if (string.IsNullOrEmpty(userInput))
            {
                MessageBox.Show("Profile name cannot be empty.");
                return;
            }

            string profilePath = Path.Combine(profilesFolderPath, $"{userInput}.json");
            DisplayConfig.SaveDisplaySettings(profilePath);
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

        inputForm.ShowDialog();

    }


    private void SaveProfile(string profilePath)
    {
        DisplayConfig.SaveDisplaySettings(profilePath);
    }

    private void DeleteProfile(string profilePath)
    {
        File.Delete(profilePath);
    }

    private void TurnOffMonitors(object? sender, EventArgs e)
    {
        DisplayConfig.DisplayToggleSleep(true);
    }

    private void OnAboutClick(object? sender, EventArgs e)
    {
        string aboutMessage =
                      "DAIRemote is a versatile display, audio, and input remote for Windows desktops. It allows users to:\n\n" +
                      "• Save and load display profiles\n" +
                      "• Cycle through audio playback devices\n" +
                      "• Use an Android phone as a keyboard and mouse input\n\n" +
                      "All of these features can be controlled remotely, providing convenience from wherever you're sitting.";

        MessageBox.Show(aboutMessage, "About DAIRemote", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void HideIcon()
    {
        trayIcon.Visible = false;
    }

    public void ShowIcon()
    {
        trayIcon.Visible = true;
    }

    private void OnShow(object sender, EventArgs e)
    {
        ShowForm();
    }

    private void ShowForm()
    {
        form.Show();
        form.WindowState = FormWindowState.Normal;
        form.Activate();
    }

    private void OnExit(object sender, EventArgs e)
    {
        HideIcon();
        Application.Exit();
    }
}
