﻿using DisplayProfileManager;
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
            ContextMenuStrip newMenu = CreateTrayMenu();

            if (form.InvokeRequired)
            {
                form.BeginInvoke((MethodInvoker)delegate
                {
                    trayIcon.ContextMenuStrip = newMenu;
                });
            }
            else
            {
                trayIcon.ContextMenuStrip = newMenu;
            }
        }

        private void PopulateTrayMenu(ContextMenuStrip menu)
        {
            // Clear existing items
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

            menu.Items.Add(loadProfilesLabel);
            menu.Items.Add(new ToolStripSeparator());
            AddDisplayProfiles(menu);
            menu.Items.Add(new ToolStripSeparator());

            // Add menu items
            ToolStripMenuItem deleteProfileMenuItem = new ToolStripMenuItem("Delete Profile", deleteProfileIcon)
            {
                AutoSize = true
            };
            deleteProfileMenuItem.DropDownItems.Add(deleteProfilesLabel);
            deleteProfileMenuItem.DropDownItems.Add(new ToolStripSeparator());
            AddDisplayProfiles(deleteProfileMenuItem);
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

        private void DeleteProfile(string profilePath)
        {
            if (File.Exists(profilePath))
            {
                File.Delete(profilePath);
                MessageBox.Show($"Profile {Path.GetFileNameWithoutExtension(profilePath)} has been deleted.", "Profile Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }

        private void AddDisplayProfiles(ContextMenuStrip menuItem)
        {
            // Assuming jsonFiles holds the paths of the profile files
            string[] jsonProfiles = Directory.GetFiles(profilesFolderPath, "*.json"); // Adjust the filter as needed

            foreach (string jsonProfile in jsonProfiles)
            {
                // Get the file name without extension for display
                string fileName = Path.GetFileNameWithoutExtension(jsonProfile);

                // Create a menu item for each JSON file
                var jsonMenuItem = new ToolStripMenuItem(fileName, monitorIcon, (sender, e) =>
                {
                    // Handle the click event to load the profile
                    DisplayProfileManager.DisplayConfig.SetDisplaySettings(jsonProfile);
                });

                // Add the profile menu item to the passed menu
                menuItem.Items.Insert(2, jsonMenuItem);
            }
        }

        private void AddDisplayProfiles(ToolStripMenuItem menuItem)
        {
            // Fetch the profiles using your existing method
            string[] jsonProfiles = Directory.GetFiles(profilesFolderPath, "*.json"); // Adjust this line as needed

            // Clear existing subitems if necessary
            // menuItem.DropDownItems.Clear();

            // Add a submenu item for each profile
            foreach (string jsonProfile in jsonProfiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(jsonProfile);

                // Create a submenu item for each profile
                ToolStripMenuItem profileItem = new ToolStripMenuItem(fileName, monitorIcon, (sender, e) =>
                {
                    DeleteProfile(jsonProfile); // Call your DeleteProfile method
                });

                // Add the profile item to the passed parent menu item
                menuItem.DropDownItems.Insert(2, profileItem);
            }
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
            trayIcon.Visible = false; // Hide tray icon
        }

        public void ShowIcon()
        {
            trayIcon.Visible = true; // Show tray icon
        }

        private void OnShow(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void ShowForm()
        {
            form.Show(); // Show the form
            form.WindowState = FormWindowState.Normal; // Restore if minimized
            form.Activate(); // Bring to front
        }

        private void OnExit(object sender, EventArgs e)
        {
            HideIcon();
            Application.Exit(); // Exit the application
        }
    }
}
