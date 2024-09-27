using static DisplayProfileManager.DisplayConfig;

namespace DAIRemote
{
    public partial class Form1 : Form
    {
        private Panel audioFormPanel;
        private Button btnShowAudioOutputs;


        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void BtnShowAudioOutputs_Click(object sender, EventArgs e)
        {
            // Clear the panel before showing the new form
            audioFormPanel.Controls.Clear();

            // Create an instance of the AudioOutputForm
            AudioOutputForm audioForm = new AudioOutputForm
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill // Fill the panel
            };

            audioFormPanel.Controls.Add(audioForm); // Add the audioForm to the panel
            audioForm.Show(); // Show the embedded form
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

         private void InitializeCustomComponents()
        {
            this.btnShowAudioOutputs = new Button();
            this.audioFormPanel = new Panel();
            this.SuspendLayout();

            // 
            // btnShowAudioOutputs
            // 
            this.btnShowAudioOutputs.Location = new System.Drawing.Point(10, 10); // Keep it in the top left corner
            this.btnShowAudioOutputs.Name = "btnShowAudioOutputs";
            this.btnShowAudioOutputs.Size = new System.Drawing.Size(150, 30);
            this.btnShowAudioOutputs.TabIndex = 0;
            this.btnShowAudioOutputs.Text = "Show Audio Outputs";
            this.btnShowAudioOutputs.UseVisualStyleBackColor = true;
            this.btnShowAudioOutputs.Click += BtnShowAudioOutputs_Click;

            // 
            // audioFormPanel
            // 
            this.audioFormPanel.Location = new System.Drawing.Point(10, 50); // Adjust this to avoid overlap
            this.audioFormPanel.Size = new System.Drawing.Size(300, 200); // Adjust size as needed
            this.audioFormPanel.BorderStyle = BorderStyle.FixedSingle; // Optional: add a border to visualize the panel

            // 
            // BtnSaveDisplayConfig
            // 
            BtnSaveDisplayConfig.AccessibleName = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.Location = new Point(180, 10); // Position it to the right of the audio button
            BtnSaveDisplayConfig.Name = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.Size = new Size(150, 30);
            BtnSaveDisplayConfig.TabIndex = 1; // Ensure tab index is sequential
            BtnSaveDisplayConfig.Text = "Save Display Config";
            BtnSaveDisplayConfig.UseVisualStyleBackColor = true;
            BtnSaveDisplayConfig.Click += BtnSaveDisplayConfig_Click;

            // 
            // Form1
            // 
            this.Controls.Add(this.btnShowAudioOutputs);
            this.Controls.Add(this.audioFormPanel);
            this.Controls.Add(BtnSaveDisplayConfig);
            this.Name = "Form1";
            this.Size = new System.Drawing.Size(320, 300); // Adjust size as needed
            this.ResumeLayout(false);
        }
    }
}
