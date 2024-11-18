using AudioManager;
using DisplayProfileManager;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using UDPServerManagerForm;

namespace DAIRemote;
public partial class DAIRemoteApplicationUI : Form
{
    private TrayIconManager trayIconManager;
    private AudioOutputForm audioForm;
    private Panel audioFormPanel;
    private AudioManager.AudioDeviceManager audioManager;
    private HotkeyManager hotkeyManager;
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

        this.Icon = new Icon("Resources/DAIRemoteLogo.ico");
        trayIconManager = new TrayIconManager(this);
        this.Load += DAIRemoteApplicationUI_Load;
        this.FormClosing += DAIRemoteApplicationUI_FormClosing;
        this.StartPosition = FormStartPosition.CenterScreen;

        SetStartupStatus();   

        hotkeyManager = new HotkeyManager();
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

    private void DAIRemoteApplicationUI_Load(object sender, EventArgs e)
    {
        this.Hide();
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

            hotkeyManager.UnregisterHotkeys();
        }
    }

    private void InitializeAudioDropDown()
    {
        this.audioFormPanel = new Panel
        {
            Location = new System.Drawing.Point(10, 320),
            Size = new System.Drawing.Size(760, 370),
        };

        audioForm = AudioOutputForm.GetInstance(audioManager);

        this.Controls.Add(this.audioFormPanel);
        audioFormPanel.Controls.Add(audioForm);
        audioForm.Show();
    }
    private void BtnAddDisplayConfig_Click(object sender, EventArgs e)
    {
        string displayProfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles");

        if (!Directory.Exists(displayProfilePath))
        {
            Directory.CreateDirectory(displayProfilePath);
        }

        TrayIconManager.SaveNewProfile(displayProfilePath);

        InitializeDisplayProfilesLayouts();  
    }

    private void InitializeDisplayProfilesLayouts()
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string[] displayProfilesDirectory = Directory.GetFiles(folderPath, "*.json");

        BtnShowLoadProfiles.Controls.Clear();
        BtnShowDeleteProfiles.Controls.Clear();


    }

    private void BtnShowLoadProfiles_Click(object sender, EventArgs e)
    {
        ShowProfilesDialog(isDelete: false);
    }

    private void BtnShowDeleteProfiles_Click(object sender, EventArgs e)
    {
        ShowProfilesDialog(isDelete: true);
    }

    private void ShowProfilesDialog(bool isDelete)
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string[] displayProfilesDirectory = Directory.GetFiles(folderPath, "*.json");

        Form profileDialog = new Form();
        profileDialog.Text = isDelete ? "Delete Profile" : "Load Profile";
        profileDialog.Size = new Size(400, 300);

        ListBox profileListBox = new ListBox();
        profileListBox.Dock = DockStyle.Fill;
        profileListBox.Items.AddRange(displayProfilesDirectory.Select(profile => Path.GetFileNameWithoutExtension(profile)).ToArray());

        profileDialog.Controls.Add(profileListBox);

        Button actionButton = new Button
        {
            Text = isDelete ? "Delete" : "Load",
            Dock = DockStyle.Bottom,
            Height = 50,              
            FlatStyle = FlatStyle.Flat,
        };

        actionButton.Click += (s, e) =>
        {
            string selectedProfile = profileListBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedProfile))
            {
                string profilePath = Path.Combine(folderPath, selectedProfile + ".json");

                if (isDelete)
                {
                    DisplayConfig.DeleteDisplaySettings(profilePath);
                    InitializeDisplayProfilesLayouts();
                }
                else
                {
                    DisplayConfig.SetDisplaySettings(profilePath);
                }
                profileDialog.Close();
            }
            else
            {
                MessageBox.Show("Please select a profile.");
            }
        };

        profileDialog.Controls.Add(actionButton);
        profileDialog.ShowDialog();
    }


    private void BtnSetAudioHotkey_Click(object sender, EventArgs e)
    {
        hotkeyManager.ShowHotkeyInput("Audio Cycling", audioManager.CycleAudioDevice);
    }

    private void BtnSetProfileHotkey_Click(object sender, EventArgs e)
    {
        hotkeyManager.ShowHotkeyInput("Load Profile", () => DisplayConfig.SetDisplaySettings("profile.json"));
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

    private void Logo_Click(object sender, EventArgs e)
    {

    }

}
