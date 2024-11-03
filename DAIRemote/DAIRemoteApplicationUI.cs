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

            this.Invoke((MethodInvoker)(() => InitializeAudioDropDown()));
        });

        // Updated the property of the form itself to start with the color
        //this.BackColor = Color.FromArgb(50, 50, 50);
        this.Icon = new Icon("Resources/DAIRemoteLogo.ico");
        trayIconManager = new TrayIconManager(this);
        this.Load += DAIRemoteApplicationUI_Load;
        this.FormClosing += DAIRemoteApplicationUI_FormClosing;
        this.StartPosition = FormStartPosition.CenterScreen;

        SetStartupStatus();   // Checks onStartup default value to set
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
        audioManager.CycleAudioDevice();
    }
}
