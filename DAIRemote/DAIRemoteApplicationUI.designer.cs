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
            DisplayDeleteProfilesLayout = new FlowLayoutPanel();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            SuspendLayout();
            // 
            // BtnSaveDisplayConfig
            // 
            BtnSaveDisplayConfig.AccessibleName = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.BackColor = Color.LightSkyBlue;
            BtnSaveDisplayConfig.Location = new Point(23, 56);
            BtnSaveDisplayConfig.Margin = new Padding(3, 4, 3, 4);
            BtnSaveDisplayConfig.Name = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.Size = new Size(229, 53);
            BtnSaveDisplayConfig.TabIndex = 0;
            BtnSaveDisplayConfig.Text = "Save Display Config";
            BtnSaveDisplayConfig.UseVisualStyleBackColor = false;
            BtnSaveDisplayConfig.Click += BtnSaveDisplayConfig_Click;
            // 
            // BtnShowAudioOutputs
            // 
            BtnShowAudioOutputs.AccessibleName = "BtnShowAudioOutputs";
            BtnShowAudioOutputs.BackColor = Color.LightSkyBlue;
            BtnShowAudioOutputs.Location = new Point(23, 117);
            BtnShowAudioOutputs.Margin = new Padding(3, 4, 3, 4);
            BtnShowAudioOutputs.Name = "BtnShowAudioOutputs";
            BtnShowAudioOutputs.Size = new Size(229, 53);
            BtnShowAudioOutputs.TabIndex = 1;
            BtnShowAudioOutputs.Text = "Show Audio Outputs";
            BtnShowAudioOutputs.UseVisualStyleBackColor = false;
            BtnShowAudioOutputs.Click += BtnShowAudioOutputs_Click;
            // 
            // profileNameTextBox
            // 
            profileNameTextBox.Location = new Point(23, 16);
            profileNameTextBox.Margin = new Padding(3, 4, 3, 4);
            profileNameTextBox.Name = "profileNameTextBox";
            profileNameTextBox.Size = new Size(228, 27);
            profileNameTextBox.TabIndex = 3;
            // 
            // checkBoxStartup
            // 
            checkBoxStartup.AutoSize = true;
            checkBoxStartup.Location = new Point(51, 563);
            checkBoxStartup.Margin = new Padding(3, 4, 3, 4);
            checkBoxStartup.Name = "checkBoxStartup";
            checkBoxStartup.Size = new Size(227, 24);
            checkBoxStartup.TabIndex = 4;
            checkBoxStartup.Text = "Launch application on startup";
            checkBoxStartup.UseVisualStyleBackColor = true;
            checkBoxStartup.CheckedChanged += checkBoxStartup_CheckedChanged;
            // 
            // DisplayLoadProfilesLayout
            // 
            DisplayLoadProfilesLayout.AutoSize = true;
            DisplayLoadProfilesLayout.Location = new Point(23, 211);
            DisplayLoadProfilesLayout.Margin = new Padding(3, 4, 3, 4);
            DisplayLoadProfilesLayout.Name = "DisplayLoadProfilesLayout";
            DisplayLoadProfilesLayout.Size = new Size(229, 133);
            DisplayLoadProfilesLayout.TabIndex = 0;
            DisplayLoadProfilesLayout.BackColor = Color.LightSkyBlue;
            // 
            // DisplayDeleteProfilesLayout
            // 
            DisplayDeleteProfilesLayout.AutoSize = true;
            DisplayDeleteProfilesLayout.Location = new Point(23, 389);
            DisplayDeleteProfilesLayout.Margin = new Padding(3, 4, 3, 4);
            DisplayDeleteProfilesLayout.Name = "DisplayDeleteProfilesLayout";
            DisplayDeleteProfilesLayout.Size = new Size(229, 133);
            DisplayDeleteProfilesLayout.TabIndex = 5;
            DisplayDeleteProfilesLayout.BackColor = Color.LightSkyBlue;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(23, 187);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(229, 27);
            textBox1.TabIndex = 6;
            textBox1.Text = "Load Display Profiles";
            textBox1.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(23, 365);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(229, 27);
            textBox2.TabIndex = 7;
            textBox2.Text = "Delete Display Profiles";
            textBox2.TextAlign = HorizontalAlignment.Center;
            // 
            // DAIRemoteApplicationUI
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(225, 246, 225);
            ClientSize = new Size(343, 600);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(DisplayDeleteProfilesLayout);
            Controls.Add(DisplayLoadProfilesLayout);
            Controls.Add(checkBoxStartup);
            Controls.Add(profileNameTextBox);
            Controls.Add(BtnSaveDisplayConfig);
            Controls.Add(BtnShowAudioOutputs);
            Margin = new Padding(3, 4, 3, 4);
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
        private FlowLayoutPanel DisplayDeleteProfilesLayout;
        private TextBox textBox1;
        private TextBox textBox2;
    }
}