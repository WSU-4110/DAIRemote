namespace DAIRemote
{
    partial class DAIRemoteApplicationUI
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
            button1 = new Button();
            profileNameTextBox = new TextBox();
            SuspendLayout();
            // 
            // BtnSaveDisplayConfig
            // 
            BtnSaveDisplayConfig.AccessibleName = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.BackColor = Color.LightSkyBlue;
            BtnSaveDisplayConfig.Location = new Point(20, 42);
            BtnSaveDisplayConfig.Name = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.Size = new Size(200, 40);
            BtnSaveDisplayConfig.TabIndex = 0;
            BtnSaveDisplayConfig.Text = "Save Display Config";
            BtnSaveDisplayConfig.UseVisualStyleBackColor = false;
            BtnSaveDisplayConfig.Click += BtnSaveDisplayConfig_Click;
            // 
            // BtnShowAudioOutputs
            // 
            BtnShowAudioOutputs.AccessibleName = "BtnShowAudioOutputs";
            BtnShowAudioOutputs.BackColor = Color.LightSkyBlue;
            BtnShowAudioOutputs.Location = new Point(20, 88);
            BtnShowAudioOutputs.Name = "BtnShowAudioOutputs";
            BtnShowAudioOutputs.Size = new Size(200, 40);
            BtnShowAudioOutputs.TabIndex = 1;
            BtnShowAudioOutputs.Text = "Show Audio Outputs";
            BtnShowAudioOutputs.UseVisualStyleBackColor = false;
            BtnShowAudioOutputs.Click += BtnShowAudioOutputs_Click;
            // 
            // button1
            // 
            button1.AccessibleName = "BtnLoadDisplayConfig";
            button1.BackColor = Color.LightSkyBlue;
            button1.Location = new Point(20, 398);
            button1.Name = "button1";
            button1.Size = new Size(200, 40);
            button1.TabIndex = 2;
            button1.Text = "Load Display Config";
            button1.UseVisualStyleBackColor = false;
            button1.Click += BtnLoadDisplayConfig_Click;
            // 
            // profileNameTextBox
            // 
            profileNameTextBox.Location = new Point(20, 12);
            profileNameTextBox.Name = "profileNameTextBox";
            profileNameTextBox.Size = new Size(200, 23);
            profileNameTextBox.TabIndex = 3;
            // 
            // DAIRemoteApplicationUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(225, 246, 225);
            ClientSize = new Size(300, 450);
            Controls.Add(profileNameTextBox);
            Controls.Add(button1);
            Controls.Add(BtnSaveDisplayConfig);
            Controls.Add(BtnShowAudioOutputs);
            Name = "DAIRemoteApplicationUI";
            Text = "DAIRemote";
            Load += DAIRemoteApplicationUI_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnSaveDisplayConfig;
        private Button BtnShowAudioOutputs;
        private Button button1;
        private TextBox profileNameTextBox;
    }
}