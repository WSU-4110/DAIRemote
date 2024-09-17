using static DisplayProfileManager.DisplayConfig;

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

        private void BtnSaveDisplayConfig_Click(object sender, EventArgs e)
        {
            try
            {
                // Define the file path where you want to save the configuration
                string filePath = "displayConfig.json";

                // Get the current display configuration
                var (pathArray, modeInfoArray, topologyId) = GetDisplayConfig();

                // Save the display configuration to a file
                SaveDisplayConfig(filePath, pathArray, modeInfoArray, topologyId);

                // Inform the user that the configuration has been saved
                MessageBox.Show($"Display configuration saved to {filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur
                MessageBox.Show($"An error occurred while saving the display configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
