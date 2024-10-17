namespace DAIRemote
{
    public class TrayIconManager
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private Form form;
        private FileSystemWatcher profileDirWatcher;
        private string profilesFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles");

        public TrayIconManager(Form form)
        {
            this.form = form;
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            trayMenu = new ContextMenuStrip
            {
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = System.Drawing.Color.White,
                ShowImageMargin = false,
                Font = new Font("Segoe UI Variable", 9, FontStyle.Regular),
            };

            profileDirWatcher = new FileSystemWatcher(@profilesFolderPath);
            profileDirWatcher.NotifyFilter = NotifyFilters.FileName;
            profileDirWatcher.Created += OnProfilesChanged;
            profileDirWatcher.Deleted += OnProfilesChanged;
            profileDirWatcher.Renamed += OnProfilesChanged;
            profileDirWatcher.EnableRaisingEvents = true;

            PopulateContextMenu();

            trayIcon = new NotifyIcon
            {
                Text = "DAIRemote",
                Icon = new Icon("Resources/DAIRemoteLogo.ico"),
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            trayIcon.DoubleClick += (s, e) => ShowForm();
        }

        private void OnProfilesChanged(object sender, FileSystemEventArgs e)
        {
            if (form.InvokeRequired)
            {
                form.BeginInvoke((MethodInvoker)delegate
                {
                    PopulateContextMenu();
                });
            }
            else
            {
                PopulateContextMenu();
            }
        }

        private void PopulateContextMenu()
        {
            // Clear existing items
            trayMenu.Items.Clear();

            if (!Directory.Exists(profilesFolderPath))
            {
                Directory.CreateDirectory(profilesFolderPath);
            }

            string[] jsonFiles = Directory.GetFiles(profilesFolderPath);

            foreach (string jsonFile in jsonFiles)
            {
                // Create menu item for each JSON file
                string fileName = Path.GetFileNameWithoutExtension(jsonFile);

                var jsonMenuItem = new ToolStripMenuItem(fileName, null, (sender, e) =>
                {
                    // Handle click event
                    DisplayProfileManager.DisplayConfig.SetDisplaySettings(jsonFile);
                });

                trayMenu.Items.Add(jsonMenuItem);
            }

            trayMenu.Items.Add(new ToolStripSeparator());

            var showMenuItem = new ToolStripMenuItem("Show", null, OnShow);
            var exitMenuItem = new ToolStripMenuItem("Exit", null, OnExit);

            trayMenu.Items.Add(showMenuItem);
            trayMenu.Items.Add(exitMenuItem);
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
