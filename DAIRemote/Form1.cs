using System;
using System.Windows.Forms;
using DisplayProfileManager;

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
        string fileName = "displayConfig";
        private void BtnSaveDisplayConfig_Click(object sender, EventArgs e)
        {
            DisplayConfig.SaveDisplaySettings(fileName + ".json");
        }
    }
}
