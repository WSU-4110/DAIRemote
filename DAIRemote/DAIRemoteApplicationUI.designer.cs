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
            checkBoxStartup = new CheckBox();
            BtnCycleAudioOutputs = new Button();
            DisplayLoadProfilesLayout = new FlowLayoutPanel();
            DisplayDeleteProfilesLayout = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // BtnSaveDisplayConfig
            // 
            BtnSaveDisplayConfig.AccessibleName = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.BackColor = Color.LightSkyBlue;
            BtnSaveDisplayConfig.Location = new Point(20, 12);
            BtnSaveDisplayConfig.Name = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.Size = new Size(200, 40);
            BtnSaveDisplayConfig.TabIndex = 0;
            BtnSaveDisplayConfig.Text = "Add Display Profile";
            BtnSaveDisplayConfig.UseVisualStyleBackColor = false;
            BtnSaveDisplayConfig.Click += BtnAddDisplayConfig_Click;
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
            // BtnCycleAudioOutputs
            // 
            BtnCycleAudioOutputs.AccessibleName = "BtnCycleAudioOutputs";
            BtnCycleAudioOutputs.BackColor = Color.LightSkyBlue;
            BtnCycleAudioOutputs.Location = new Point(20, 58);
            BtnCycleAudioOutputs.Name = "BtnCycleAudioOutputs";
            BtnCycleAudioOutputs.Size = new Size(200, 40);
            BtnCycleAudioOutputs.TabIndex = 5;
            BtnCycleAudioOutputs.Text = "Cycle Audio Devices";
            BtnCycleAudioOutputs.UseVisualStyleBackColor = false;
            BtnCycleAudioOutputs.Click += BtnCycleAudioOutputs_Click;
            // 
            // DisplayLoadProfilesLayout
            // 
            DisplayLoadProfilesLayout.Location = new Point(283, 12);
            DisplayLoadProfilesLayout.Name = "DisplayLoadProfilesLayout";
            DisplayLoadProfilesLayout.Size = new Size(476, 178);
            DisplayLoadProfilesLayout.TabIndex = 6;
            // 
            // DisplayDeleteProfilesLayout
            // 
            DisplayDeleteProfilesLayout.Location = new Point(283, 196);
            DisplayDeleteProfilesLayout.Name = "DisplayDeleteProfilesLayout";
            DisplayDeleteProfilesLayout.Size = new Size(476, 189);
            DisplayDeleteProfilesLayout.TabIndex = 7;
            // 
            // DAIRemoteApplicationUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(225, 246, 225);
            ClientSize = new Size(771, 395);
            Controls.Add(DisplayDeleteProfilesLayout);
            Controls.Add(DisplayLoadProfilesLayout);
            Controls.Add(BtnCycleAudioOutputs);
            Controls.Add(checkBoxStartup);
            Controls.Add(BtnSaveDisplayConfig);
            Name = "DAIRemoteApplicationUI";
            Text = "DAIRemote";
            Load += DAIRemoteApplicationUI_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnSaveDisplayConfig;
        private CheckBox checkBoxStartup;
        private Button BtnCycleAudioOutputs;
        private FlowLayoutPanel DisplayLoadProfilesLayout;
        private FlowLayoutPanel DisplayDeleteProfilesLayout;
    }
}