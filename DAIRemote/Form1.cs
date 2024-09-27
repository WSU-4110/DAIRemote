using DisplayProfileManager;

namespace DAIRemote
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        string fileName = "displayConfig";
        private void BtnSaveDisplayConfig_Click(object sender, EventArgs e)
        {
            DisplayConfig.SaveDisplaySettings(fileName + ".json");
        }
    }
}
