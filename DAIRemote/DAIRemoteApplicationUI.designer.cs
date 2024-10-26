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
            profileNameTextBox = new TextBox();
            checkBoxStartup = new CheckBox();
            DisplayLoadProfilesLayout = new FlowLayoutPanel();
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
            // profileNameTextBox
            // 
            profileNameTextBox.Location = new Point(20, 12);
            profileNameTextBox.Name = "profileNameTextBox";
            profileNameTextBox.Size = new Size(200, 23);
            profileNameTextBox.TabIndex = 3;
            // 
            // checkBoxStartup
            // 
            checkBoxStartup.AutoSize = true;
            checkBoxStartup.Location = new Point(24, 366);
            checkBoxStartup.Name = "checkBoxStartup";
            checkBoxStartup.Size = new Size(184, 19);
            checkBoxStartup.TabIndex = 4;
            checkBoxStartup.Text = "Launch application on startup";
            checkBoxStartup.UseVisualStyleBackColor = true;
            checkBoxStartup.CheckedChanged += checkBoxStartup_CheckedChanged;
            // 
            // DisplayLoadProfilesLayout
            // 
            DisplayLoadProfilesLayout.AutoSize = true;
            DisplayLoadProfilesLayout.Location = new Point(10, 250);
            DisplayLoadProfilesLayout.Name = "DisplayLoadProfilesLayout";
            DisplayLoadProfilesLayout.Size = new Size(200, 100);
            DisplayLoadProfilesLayout.TabIndex = 0;
            // 
            // DAIRemoteApplicationUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(225, 246, 225);
            ClientSize = new Size(300, 450);
            Controls.Add(DisplayLoadProfilesLayout);
            Controls.Add(checkBoxStartup);
            Controls.Add(profileNameTextBox);
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
        private TextBox profileNameTextBox;
        private CheckBox checkBoxStartup;
        private FlowLayoutPanel DisplayLoadProfilesLayout;
    }
}