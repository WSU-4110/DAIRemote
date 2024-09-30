namespace DAIRemote
{
    partial class Form1
    {
       
        private System.ComponentModel.IContainer components = null;

        
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        
        private void InitializeComponent()
        {

            BtnSaveDisplayConfig = new Button();
            BtnShowAudioOutputs = new Button(); 
            SuspendLayout();

            BtnSaveDisplayConfig.AccessibleName = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.Location = new Point(20, 10);
            BtnSaveDisplayConfig.Name = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.Size = new Size(200, 40); 
            BtnSaveDisplayConfig.TabIndex = 0;
            BtnSaveDisplayConfig.Text = "Save Display Config";
            BtnSaveDisplayConfig.BackColor = Color.LightSkyBlue; 
            BtnSaveDisplayConfig.Click += BtnSaveDisplayConfig_Click;

            BtnShowAudioOutputs.AccessibleName = "BtnShowAudioOutputs";
            BtnShowAudioOutputs.Location = new Point(20, 70); 
            BtnShowAudioOutputs.Name = "BtnShowAudioOutputs";
            BtnShowAudioOutputs.Size = new Size(200, 40); 
            BtnShowAudioOutputs.TabIndex = 1;
            BtnShowAudioOutputs.Text = "Show Audio Outputs";
            BtnShowAudioOutputs.BackColor = Color.LightSkyBlue; 
            BtnShowAudioOutputs.Click += BtnShowAudioOutputs_Click;

           
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(300, 450);
            BackColor = Color.FromArgb(50, 50, 50); 
            Controls.Add(BtnSaveDisplayConfig);
            Controls.Add(BtnShowAudioOutputs); 
            Name = "Form1";
            Text = "DAIRemote";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button BtnSaveDisplayConfig;
        private Button BtnShowAudioOutputs; 
    }
}