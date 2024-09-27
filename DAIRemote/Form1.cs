using System;
using System.Windows.Forms;
using static DisplayProfileManager.DisplayConfig;

namespace DAIRemote
{
    public partial class Form1 : Form
    {
        private TrayIconManager trayIconManager;

        public Form1()
        {
            InitializeComponent();
            trayIconManager = new TrayIconManager(this);
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = false;
                trayIconManager.HideIcon();
            }
        }

        private void BtnSaveDisplayConfig_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = "displayConfig.json";

                var (pathArray, modeInfoArray, topologyId) = GetDisplayConfig();
                SaveDisplayConfig(filePath, pathArray, modeInfoArray, topologyId);

                MessageBox.Show($"Display configuration saved to {filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the display configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
