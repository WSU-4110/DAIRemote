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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DAIRemoteApplicationUI));
            this.BtnSaveDisplayConfig = new Button();
            this.checkBoxStartup = new CheckBox();
            this.BtnCycleAudioOutputs = new Button();
            this.DisplayLoadProfilesLayout = new FlowLayoutPanel();
            this.DisplayDeleteProfilesLayout = new FlowLayoutPanel();
            this.Logo = new PictureBox();
            this.LogoName = new TextBox();
            this.Description = new TextBox();
            this.LoadProfile = new TextBox();
            this.DeleteProfile = new TextBox();
            this.button1 = new Button();
            this.button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)this.Logo).BeginInit();
            this.SuspendLayout();
            // 
            // BtnSaveDisplayConfig
            // 
            this.BtnSaveDisplayConfig.AccessibleName = "BtnSaveDisplayConfig";
            this.BtnSaveDisplayConfig.BackColor = Color.LightSkyBlue;
            this.BtnSaveDisplayConfig.Location = new Point(20, 209);
            this.BtnSaveDisplayConfig.Name = "BtnSaveDisplayConfig";
            this.BtnSaveDisplayConfig.Size = new Size(200, 40);
            this.BtnSaveDisplayConfig.TabIndex = 0;
            this.BtnSaveDisplayConfig.Text = "Add Display Profile";
            this.BtnSaveDisplayConfig.UseVisualStyleBackColor = false;
            this.BtnSaveDisplayConfig.Click += this.BtnAddDisplayConfig_Click;
            // 
            // checkBoxStartup
            // 
            this.checkBoxStartup.AutoSize = true;
            this.checkBoxStartup.ForeColor = SystemColors.Control;
            this.checkBoxStartup.Location = new Point(20, 393);
            this.checkBoxStartup.Name = "checkBoxStartup";
            this.checkBoxStartup.Size = new Size(184, 19);
            this.checkBoxStartup.TabIndex = 4;
            this.checkBoxStartup.Text = "Launch application on startup";
            this.checkBoxStartup.UseVisualStyleBackColor = true;
            this.checkBoxStartup.CheckedChanged += this.CheckBoxStartup_CheckedChanged;
            // 
            // BtnCycleAudioOutputs
            // 
            this.BtnCycleAudioOutputs.AccessibleName = "BtnCycleAudioOutputs";
            this.BtnCycleAudioOutputs.BackColor = Color.LightSkyBlue;
            this.BtnCycleAudioOutputs.Location = new Point(20, 255);
            this.BtnCycleAudioOutputs.Name = "BtnCycleAudioOutputs";
            this.BtnCycleAudioOutputs.Size = new Size(200, 40);
            this.BtnCycleAudioOutputs.TabIndex = 5;
            this.BtnCycleAudioOutputs.Text = "Cycle Audio Devices";
            this.BtnCycleAudioOutputs.UseVisualStyleBackColor = false;
            this.BtnCycleAudioOutputs.Click += this.BtnCycleAudioOutputs_Click;
            // 
            // DisplayLoadProfilesLayout
            // 
            this.DisplayLoadProfilesLayout.AutoScroll = true;
            this.DisplayLoadProfilesLayout.Location = new Point(283, 12);
            this.DisplayLoadProfilesLayout.Name = "DisplayLoadProfilesLayout";
            this.DisplayLoadProfilesLayout.Size = new Size(476, 178);
            this.DisplayLoadProfilesLayout.TabIndex = 6;
            // 
            // DisplayDeleteProfilesLayout
            // 
            this.DisplayDeleteProfilesLayout.AutoScroll = true;
            this.DisplayDeleteProfilesLayout.Location = new Point(283, 196);
            this.DisplayDeleteProfilesLayout.Name = "DisplayDeleteProfilesLayout";
            this.DisplayDeleteProfilesLayout.Size = new Size(476, 178);
            this.DisplayDeleteProfilesLayout.TabIndex = 7;
            // 
            // Logo
            // 
            this.Logo.Image = Properties.Resources.DAIRemoteLogo;
            this.Logo.Location = new Point(0, 0);
            this.Logo.Margin = new Padding(3, 2, 3, 2);
            this.Logo.Name = "Logo";
            this.Logo.Size = new Size(98, 92);
            this.Logo.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Logo.TabIndex = 8;
            this.Logo.TabStop = false;
            // 
            // LogoName
            // 
            this.LogoName.BackColor = Color.FromArgb(50, 50, 50);
            this.LogoName.BorderStyle = BorderStyle.None;
            this.LogoName.Font = new Font("Cascadia Code", 20F, FontStyle.Bold);
            this.LogoName.ForeColor = Color.White;
            this.LogoName.Location = new Point(104, 23);
            this.LogoName.Margin = new Padding(3, 2, 3, 2);
            this.LogoName.Name = "LogoName";
            this.LogoName.ReadOnly = true;
            this.LogoName.Size = new Size(150, 31);
            this.LogoName.TabIndex = 9;
            this.LogoName.Text = "DAIRemote";
            // 
            // Description
            // 
            this.Description.BackColor = Color.FromArgb(50, 50, 50);
            this.Description.BorderStyle = BorderStyle.None;
            this.Description.ForeColor = Color.White;
            this.Description.Location = new Point(12, 91);
            this.Description.Margin = new Padding(3, 2, 3, 2);
            this.Description.Multiline = true;
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Size = new Size(242, 108);
            this.Description.TabIndex = 10;
            this.Description.Text = resources.GetString("Description.Text");
            // 
            // LoadProfile
            // 
            this.LoadProfile.BackColor = Color.FromArgb(50, 50, 50);
            this.LoadProfile.BorderStyle = BorderStyle.None;
            this.LoadProfile.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point, 0);
            this.LoadProfile.ForeColor = Color.White;
            this.LoadProfile.Location = new Point(472, 0);
            this.LoadProfile.Margin = new Padding(3, 2, 3, 2);
            this.LoadProfile.Name = "LoadProfile";
            this.LoadProfile.ReadOnly = true;
            this.LoadProfile.Size = new Size(109, 16);
            this.LoadProfile.TabIndex = 11;
            this.LoadProfile.Text = "Load Profile";
            this.LoadProfile.TextAlign = HorizontalAlignment.Center;
            // 
            // DeleteProfile
            // 
            this.DeleteProfile.BackColor = Color.FromArgb(50, 50, 50);
            this.DeleteProfile.BorderStyle = BorderStyle.None;
            this.DeleteProfile.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point, 0);
            this.DeleteProfile.ForeColor = Color.White;
            this.DeleteProfile.Location = new Point(472, 184);
            this.DeleteProfile.Margin = new Padding(3, 2, 3, 2);
            this.DeleteProfile.Name = "DeleteProfile";
            this.DeleteProfile.ReadOnly = true;
            this.DeleteProfile.Size = new Size(109, 16);
            this.DeleteProfile.TabIndex = 12;
            this.DeleteProfile.Text = "Delete Profile";
            this.DeleteProfile.TextAlign = HorizontalAlignment.Center;
            // 
            // button1
            // 
            this.button1.AccessibleName = "BtnSetAudioCycleHotkey";
            this.button1.BackColor = Color.LightSkyBlue;
            this.button1.Location = new Point(20, 301);
            this.button1.Name = "button1";
            this.button1.Size = new Size(200, 40);
            this.button1.TabIndex = 13;
            this.button1.Text = "Set Audio Cycle Hotkey";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += this.BtnSetAudioHotkey_Click;
            // 
            // button2
            // 
            this.button2.AccessibleName = "BtnSetDisplayProfileHotkey";
            this.button2.BackColor = Color.LightSkyBlue;
            this.button2.Location = new Point(20, 347);
            this.button2.Name = "button2";
            this.button2.Size = new Size(200, 40);
            this.button2.TabIndex = 14;
            this.button2.Text = "Set Display Profile Hotkeys";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += this.BtnSetDisplayProfileHotkey_click;
            // 
            // DAIRemoteApplicationUI
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(50, 50, 50);
            this.ClientSize = new Size(771, 453);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.DeleteProfile);
            this.Controls.Add(this.LoadProfile);
            this.Controls.Add(this.Description);
            this.Controls.Add(this.LogoName);
            this.Controls.Add(this.Logo);
            this.Controls.Add(this.DisplayDeleteProfilesLayout);
            this.Controls.Add(this.DisplayLoadProfilesLayout);
            this.Controls.Add(this.BtnCycleAudioOutputs);
            this.Controls.Add(this.checkBoxStartup);
            this.Controls.Add(this.BtnSaveDisplayConfig);
            this.Name = "DAIRemoteApplicationUI";
            this.Text = "DAIRemote";
            ((System.ComponentModel.ISupportInitialize)this.Logo).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Button BtnSaveDisplayConfig;
        private CheckBox checkBoxStartup;
        private Button BtnCycleAudioOutputs;
        private FlowLayoutPanel DisplayLoadProfilesLayout;
        private FlowLayoutPanel DisplayDeleteProfilesLayout;
        private PictureBox Logo;
        private TextBox LogoName;
        private TextBox Description;
        private TextBox LoadProfile;
        private TextBox DeleteProfile;
        private Button button1;
        private Button button2;
    }
}