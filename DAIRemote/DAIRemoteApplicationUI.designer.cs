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
            this.AudioCycleHotkey = new Button();
            this.DisplayProfileHotkey = new Button();
            ((System.ComponentModel.ISupportInitialize)this.Logo).BeginInit();
            this.SuspendLayout();
            // 
            // BtnSaveDisplayConfig
            // 
            this.BtnSaveDisplayConfig.AccessibleName = "BtnSaveDisplayConfig";
            this.BtnSaveDisplayConfig.BackColor = Color.LightSkyBlue;
            this.BtnSaveDisplayConfig.Location = new Point(23, 287);
            this.BtnSaveDisplayConfig.Margin = new Padding(3, 4, 3, 4);
            this.BtnSaveDisplayConfig.Name = "BtnSaveDisplayConfig";
            this.BtnSaveDisplayConfig.Size = new Size(229, 54);
            this.BtnSaveDisplayConfig.TabIndex = 0;
            this.BtnSaveDisplayConfig.Text = "Add Display Profile";
            this.BtnSaveDisplayConfig.UseVisualStyleBackColor = false;
            this.BtnSaveDisplayConfig.Click += this.BtnAddDisplayConfig_Click;
            // 
            // checkBoxStartup
            // 
            this.checkBoxStartup.AutoSize = true;
            this.checkBoxStartup.ForeColor = SystemColors.Control;
            this.checkBoxStartup.Location = new Point(23, 535);
            this.checkBoxStartup.Margin = new Padding(3, 4, 3, 4);
            this.checkBoxStartup.Name = "checkBoxStartup";
            this.checkBoxStartup.Size = new Size(227, 24);
            this.checkBoxStartup.TabIndex = 4;
            this.checkBoxStartup.Text = "Launch application on startup";
            this.checkBoxStartup.UseVisualStyleBackColor = true;
            this.checkBoxStartup.CheckedChanged += this.CheckBoxStartup_CheckedChanged;
            // 
            // BtnCycleAudioOutputs
            // 
            this.BtnCycleAudioOutputs.AccessibleName = "BtnCycleAudioOutputs";
            this.BtnCycleAudioOutputs.BackColor = Color.LightSkyBlue;
            this.BtnCycleAudioOutputs.Location = new Point(23, 349);
            this.BtnCycleAudioOutputs.Margin = new Padding(3, 4, 3, 4);
            this.BtnCycleAudioOutputs.Name = "BtnCycleAudioOutputs";
            this.BtnCycleAudioOutputs.Size = new Size(229, 54);
            this.BtnCycleAudioOutputs.TabIndex = 5;
            this.BtnCycleAudioOutputs.Text = "Cycle Audio Devices";
            this.BtnCycleAudioOutputs.UseVisualStyleBackColor = false;
            this.BtnCycleAudioOutputs.Click += this.BtnCycleAudioOutputs_Click;
            // 
            // DisplayLoadProfilesLayout
            // 
            this.DisplayLoadProfilesLayout.AutoScroll = true;
            this.DisplayLoadProfilesLayout.Location = new Point(323, 52);
            this.DisplayLoadProfilesLayout.Margin = new Padding(3, 4, 3, 4);
            this.DisplayLoadProfilesLayout.Name = "DisplayLoadProfilesLayout";
            this.DisplayLoadProfilesLayout.Size = new Size(544, 238);
            this.DisplayLoadProfilesLayout.TabIndex = 6;
            // 
            // DisplayDeleteProfilesLayout
            // 
            this.DisplayDeleteProfilesLayout.AutoScroll = true;
            this.DisplayDeleteProfilesLayout.Location = new Point(323, 349);
            this.DisplayDeleteProfilesLayout.Margin = new Padding(3, 4, 3, 4);
            this.DisplayDeleteProfilesLayout.Name = "DisplayDeleteProfilesLayout";
            this.DisplayDeleteProfilesLayout.Size = new Size(544, 238);
            this.DisplayDeleteProfilesLayout.TabIndex = 7;
            // 
            // Logo
            // 
            this.Logo.Image = Properties.Resources.DAIRemoteLogo;
            this.Logo.Location = new Point(0, 0);
            this.Logo.Margin = new Padding(3, 2, 3, 2);
            this.Logo.Name = "Logo";
            this.Logo.Size = new Size(112, 122);
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
            this.LogoName.Location = new Point(119, 30);
            this.LogoName.Margin = new Padding(3, 2, 3, 2);
            this.LogoName.Name = "LogoName";
            this.LogoName.ReadOnly = true;
            this.LogoName.Size = new Size(171, 39);
            this.LogoName.TabIndex = 9;
            this.LogoName.Text = "DAIRemote";
            // 
            // Description
            // 
            this.Description.BackColor = Color.FromArgb(50, 50, 50);
            this.Description.BorderStyle = BorderStyle.None;
            this.Description.ForeColor = Color.White;
            this.Description.Location = new Point(14, 121);
            this.Description.Margin = new Padding(3, 2, 3, 2);
            this.Description.Multiline = true;
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Size = new Size(277, 160);
            this.Description.TabIndex = 10;
            this.Description.Text = resources.GetString("Description.Text");
            // 
            // LoadProfile
            // 
            this.LoadProfile.BackColor = Color.FromArgb(50, 50, 50);
            this.LoadProfile.BorderStyle = BorderStyle.None;
            this.LoadProfile.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point, 0);
            this.LoadProfile.ForeColor = Color.LightSkyBlue;
            this.LoadProfile.Location = new Point(518, 26);
            this.LoadProfile.Margin = new Padding(3, 2, 3, 2);
            this.LoadProfile.Name = "LoadProfile";
            this.LoadProfile.ReadOnly = true;
            this.LoadProfile.Size = new Size(125, 20);
            this.LoadProfile.TabIndex = 11;
            this.LoadProfile.Text = "Load Profile";
            this.LoadProfile.TextAlign = HorizontalAlignment.Center;
            // 
            // DeleteProfile
            // 
            this.DeleteProfile.BackColor = Color.FromArgb(50, 50, 50);
            this.DeleteProfile.BorderStyle = BorderStyle.None;
            this.DeleteProfile.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point, 0);
            this.DeleteProfile.ForeColor = Color.Red;
            this.DeleteProfile.Location = new Point(518, 323);
            this.DeleteProfile.Margin = new Padding(3, 2, 3, 2);
            this.DeleteProfile.Name = "DeleteProfile";
            this.DeleteProfile.ReadOnly = true;
            this.DeleteProfile.Size = new Size(125, 20);
            this.DeleteProfile.TabIndex = 12;
            this.DeleteProfile.Text = "Delete Profile";
            this.DeleteProfile.TextAlign = HorizontalAlignment.Center;
            // 
            // AudioCycleHotkey
            // 
            this.AudioCycleHotkey.AccessibleName = "BtnSetAudioCycleHotkey";
            this.AudioCycleHotkey.BackColor = Color.LightSkyBlue;
            this.AudioCycleHotkey.Location = new Point(23, 411);
            this.AudioCycleHotkey.Margin = new Padding(3, 4, 3, 4);
            this.AudioCycleHotkey.Name = "AudioCycleHotkey";
            this.AudioCycleHotkey.Size = new Size(229, 54);
            this.AudioCycleHotkey.TabIndex = 13;
            this.AudioCycleHotkey.Text = "Set Audio Cycle Hotkey";
            this.AudioCycleHotkey.UseVisualStyleBackColor = false;
            this.AudioCycleHotkey.Click += this.BtnSetAudioHotkey_Click;
            // 
            // DisplayProfileHotkey
            // 
            this.DisplayProfileHotkey.AccessibleName = "BtnSetDisplayProfileHotkey";
            this.DisplayProfileHotkey.BackColor = Color.LightSkyBlue;
            this.DisplayProfileHotkey.Location = new Point(23, 473);
            this.DisplayProfileHotkey.Margin = new Padding(3, 4, 3, 4);
            this.DisplayProfileHotkey.Name = "DisplayProfileHotkey";
            this.DisplayProfileHotkey.Size = new Size(229, 54);
            this.DisplayProfileHotkey.TabIndex = 14;
            this.DisplayProfileHotkey.Text = "Set Display Profile Hotkeys";
            this.DisplayProfileHotkey.UseVisualStyleBackColor = false;
            this.DisplayProfileHotkey.Click += this.BtnSetDisplayProfileHotkey_click;
            // 
            // DAIRemoteApplicationUI
            // 
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(50, 50, 50);
            this.ClientSize = new Size(881, 604);
            this.Controls.Add(this.DisplayProfileHotkey);
            this.Controls.Add(this.AudioCycleHotkey);
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
            this.Margin = new Padding(3, 4, 3, 4);
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
        private Button AudioCycleHotkey;
        private Button DisplayProfileHotkey;
    }
}