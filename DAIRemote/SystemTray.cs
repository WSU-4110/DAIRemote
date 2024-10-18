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
        private MonitorControl monitorControl;
        private HashSet<string> addedProfiles = new HashSet<string>();

        public TrayIconManager(Form form)
        {
            this.form = form;
            InitializeTrayIcon();
            monitorControl = new MonitorControl(trayIcon);
        }

        private void InitializeTrayIcon()
        {
            trayMenu = CreateContextMenu();
            trayIcon = new NotifyIcon
            {
                Text = "DAIRemote",
                Icon = new Icon("Resources/DAIRemoteLogo.ico"),
                ContextMenuStrip = trayMenu,
                Visible = true
            };
            trayIcon.DoubleClick += (s, e) => ShowForm();
        }

        private ContextMenuStrip CreateContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip
            {
                ForeColor = System.Drawing.Color.Black,
                ShowImageMargin = true,
                Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
            };
            ToolStripLabel loadProfilesLabel = new ToolStripLabel("Load Profiles")
            {
                Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
                ForeColor = Color.Gray
            };

            menu.Items.Add(loadProfilesLabel);
            menu.Items.Add(new ToolStripSeparator());
            AddDisplayProfiles(menu);

            Image aboutIcon = Image.FromFile("Resources/AboutIcon.ico");
            Image exitIcon = Image.FromFile("Resources/ExitIcon.ico");
            Image turnOffAllMonitorsIcon = Image.FromFile("Resources/TurnOffAllMonitors.ico");
            Image deleteProfileIcon = Image.FromFile("Resources/DeleteProfile.ico");

            ToolStripMenuItem deleteProfileMenuItem = new ToolStripMenuItem("Delete Profile", deleteProfileIcon);
            AddDeleteProfiles(deleteProfileMenuItem);
            menu.Items.Add(deleteProfileMenuItem);

            var turnOffAllMonitorsItem = new ToolStripMenuItem("Turn Off All Monitors", turnOffAllMonitorsIcon, TurnOffMonitors);
            var aboutMenuItem = new ToolStripMenuItem("About", aboutIcon, onAboutClick);
            var exitMenuItem = new ToolStripMenuItem("Exit", exitIcon, OnExit);

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(turnOffAllMonitorsItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(aboutMenuItem);
            menu.Items.Add(exitMenuItem);

            return menu;
        }

        private void AddDeleteProfiles(ToolStripMenuItem deleteProfileMenuItem)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appDataPath, "DAIRemote");
            Image MonitorIcon = Image.FromFile("Resources/MonitorIcon.ico");

            if (Directory.Exists(folderPath))
            {
                string[] profiles = Directory.GetFiles(folderPath, "*.json");

                foreach (string profile in profiles)
                {
                    string profileName = Path.GetFileNameWithoutExtension(profile);
                    var deleteProfileSubItem = new ToolStripMenuItem(profileName, MonitorIcon, (s, e) => DeleteProfile(profile));
                    deleteProfileMenuItem.DropDownItems.Add(deleteProfileSubItem);
                }
            }
        }

        private void DeleteProfile(string profilePath)
        {
            if (File.Exists(profilePath))
            {
                File.Delete(profilePath);
                MessageBox.Show($"Profile {Path.GetFileNameWithoutExtension(profilePath)} has been deleted.", "Profile Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                addedProfiles.Clear();
                trayMenu.Items.Clear();
                trayMenu = CreateContextMenu();
                trayIcon.ContextMenuStrip = trayMenu;
            }
        }

        public void RefreshDeleteProfileMenu()
        {
            foreach (ToolStripMenuItem item in trayMenu.Items.OfType<ToolStripMenuItem>())
            {
                if (item.Text == "Delete Profile")
                {
                    item.DropDownItems.Clear();
                    AddDeleteProfiles(item);
                    break;
                }
            }
        }

        public void AddDisplayProfiles(ContextMenuStrip menu)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appDataPath, "DAIRemote");
            Image MonitorIcon = Image.FromFile("Resources/MonitorIcon.ico");

            if (Directory.Exists(folderPath))
            {
                string[] profiles = Directory.GetFiles(folderPath, "*.json");

                foreach (string profile in profiles)
                {
                    string profileName = Path.GetFileNameWithoutExtension(profile);

                    if (!addedProfiles.Contains(profileName))
                    {
                        var profileMenuItem = new ToolStripMenuItem(profileName, MonitorIcon, (s, e) => MonitorControl.LoadDisplayProfile(profile));
                        menu.Items.Insert(2, profileMenuItem);
                        addedProfiles.Add(profileName);
                    }
                }
            }
        }

        private void TurnOffMonitors(object? sender, EventArgs e)
        {
            monitorControl.TurnOffMonitors();
        }

        private void onAboutClick(object? sender, EventArgs e)
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

        public ContextMenuStrip GetContextMenu()
        {
            return trayMenu;
        }

    }
}
