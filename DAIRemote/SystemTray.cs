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
        private Image plusIcon;

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
            plusIcon = Image.FromFile("Resources/AddProfile.ico");

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

            ToolStripMenuItem saveProfilesLabel = new ToolStripMenuItem("Select Profile To Save")
            {
                Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                Enabled = false,
            };

            ToolStripMenuItem addNewProfile = new ToolStripMenuItem("Add New Profile", plusIcon, (sender, e) =>
            {
                SaveNewProfile(profilesFolderPath);
            });

            ToolStripMenuItem saveProfileMenuItem = new ToolStripMenuItem("Save Profile", saveProfileIcon);
            ToolStripMenuItem deleteProfileMenuItem = new ToolStripMenuItem("Delete Profile", deleteProfileIcon);

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
            saveProfileMenuItem.DropDownItems.Insert(2, addNewProfile);
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

        private void SaveNewProfile(string profilesFolderPath)
        {
            Form inputForm = new Form
            {
                Width = 400,
                Height = 150,
                Text = "Save New Profile",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false
            };

            Label promptLabel = new Label
            {
                Left = 20,
                Top = 20,
                Text = "Please enter the profile name:",
                AutoSize = true
            };

            TextBox inputBox = new TextBox
            {
                Left = 20,
                Top = 50,
                Width = 350
            };

            Button okButton = new Button
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

            Button cancelButton = new Button
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
