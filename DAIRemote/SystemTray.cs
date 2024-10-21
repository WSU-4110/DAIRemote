using DisplayProfileManager;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DAIRemote
{
    public class TrayIconManager
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private Form form;
        private FileSystemWatcher profileDirWatcher;
        private string profilesFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles");
        private DisplayConfig displayConfig;

        private Image aboutIcon;
        private Image DAIRemoteLogo;
        private Image deleteProfileIcon;
        private Image exitIcon;
        private Image monitorIcon;
        private Image saveProfileIcon;
        private Image turnOffAllMonitorsIcon;

        public TrayIconManager(Form form)
        {
            this.form = form;
            displayConfig = new DisplayConfig();

            aboutIcon = Image.FromFile("Resources/About.ico");
            DAIRemoteLogo = Image.FromFile("Resources/DAIRemoteLogo.ico");
            deleteProfileIcon = Image.FromFile("Resources/DeleteProfile.ico");
            exitIcon = Image.FromFile("Resources/Exit.ico");
            monitorIcon = Image.FromFile("Resources/Monitor.ico");
            saveProfileIcon = Image.FromFile("Resources/SaveProfile.ico");
            turnOffAllMonitorsIcon = Image.FromFile("Resources/TurnOffAllMonitors.ico");

            if (!Directory.Exists(profilesFolderPath))
            {
                Directory.CreateDirectory(profilesFolderPath); 
            }

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

            profileDirWatcher = new FileSystemWatcher(@profilesFolderPath);
            profileDirWatcher.NotifyFilter = NotifyFilters.FileName;
            profileDirWatcher.Created += OnProfilesChanged;
            profileDirWatcher.Deleted += OnProfilesChanged;
            profileDirWatcher.Renamed += OnProfilesChanged;
            profileDirWatcher.EnableRaisingEvents = true;

            trayIcon.DoubleClick += (s, e) => ShowForm();
        }

        private ContextMenuStrip CreateTrayMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip
            {
                ForeColor = System.Drawing.Color.Black,
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

            ToolStripLabel loadProfilesLabel = new ToolStripLabel("Loaded Profiles")
            {
                Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                Enabled = false,
            };

            ToolStripMenuItem deleteProfilesLabel = new ToolStripMenuItem("Select Profile To Delete")
            {
                Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                Enabled = false,
            };

            ToolStripMenuItem saveProfilesLabel = new ToolStripMenuItem("Select Profile To Override")
            {
                Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                Enabled = false,
            };

            ToolStripMenuItem saveProfileMenuItem = new ToolStripMenuItem("Save Profile", saveProfileIcon);
            saveProfileMenuItem.DropDownItems.Clear();

            ToolStripMenuItem deleteProfileMenuItem = new ToolStripMenuItem("Delete Profile", deleteProfileIcon);
            deleteProfileMenuItem.DropDownItems.Clear();

            menu.Items.Add(loadProfilesLabel);
            menu.Items.Add(new ToolStripSeparator());

            string[] jsonProfiles = Directory.GetFiles(profilesFolderPath, "*.json");

            foreach (string jsonProfile in jsonProfiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(jsonProfile);
                var jsonMenuItem = new ToolStripMenuItem(fileName, monitorIcon, (sender, e) =>
                {
                    DisplayProfileManager.DisplayConfig.SetDisplaySettings(jsonProfile);
                });

                menu.Items.Insert(2, jsonMenuItem);

                ToolStripMenuItem profileDeleteItem = new ToolStripMenuItem(fileName, monitorIcon, (sender, e) =>
                {
                    DeleteProfile(jsonProfile);
                });

                ToolStripMenuItem profileSaveItem = new ToolStripMenuItem(fileName, monitorIcon, (sender, e) =>
                {
                    SaveProfile(jsonProfile);
                });

                deleteProfileMenuItem.DropDownItems.Add(profileDeleteItem);
                saveProfileMenuItem.DropDownItems.Add(profileSaveItem);
            }

            menu.Items.Add(new ToolStripSeparator());
            deleteProfileMenuItem.DropDownItems.Insert(0, deleteProfilesLabel);
            deleteProfileMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());
            saveProfileMenuItem.DropDownItems.Insert(0, saveProfilesLabel);
            saveProfileMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());
            menu.Items.Add(saveProfileMenuItem);
            menu.Items.Add(deleteProfileMenuItem);

            ToolStripMenuItem turnOffAllMonitorsItem = new ToolStripMenuItem("Turn Off All Monitors", turnOffAllMonitorsIcon, TurnOffMonitors);
            ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About", aboutIcon, OnAboutClick);
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit", exitIcon, OnExit);

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(turnOffAllMonitorsItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(aboutMenuItem);
            menu.Items.Add(exitMenuItem);
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
            displayConfig.TurnOffMonitors();
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
}
