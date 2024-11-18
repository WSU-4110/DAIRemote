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
            this.BtnShowLoadProfiles = new Button();
            this.BtnShowDeleteProfiles = new Button();
            this.BtnCycleAudioOutputs = new Button();
            this.BtnSetProfileHotkey = new Button();
            this.BtnSetAudioHotkey = new Button();
            this.Logo = new PictureBox();
            this.LogoName = new TextBox();
            this.Description = new TextBox();
            ((System.ComponentModel.ISupportInitialize)this.Logo).BeginInit();
            this.SuspendLayout();
            // 
            // BtnSaveDisplayConfig
            // 
            this.BtnSaveDisplayConfig.AccessibleName = "BtnSaveDisplayConfig";
            this.BtnSaveDisplayConfig.BackColor = Color.LightSkyBlue;
            this.BtnSaveDisplayConfig.Location = new Point(29, 281);
            this.BtnSaveDisplayConfig.Margin = new Padding(4, 5, 4, 5);
            this.BtnSaveDisplayConfig.Name = "BtnSaveDisplayConfig";
            this.BtnSaveDisplayConfig.Size = new Size(286, 66);
            this.BtnSaveDisplayConfig.TabIndex = 0;
            this.BtnSaveDisplayConfig.Text = "Add Display Profile";
            this.BtnSaveDisplayConfig.UseVisualStyleBackColor = false;
            this.BtnSaveDisplayConfig.Click += new System.EventHandler(this.BtnAddDisplayConfig_Click);

            // 
            // checkBoxStartup
            // 
            this.checkBoxStartup.AutoSize = true;
            this.checkBoxStartup.ForeColor = SystemColors.Control;
            this.checkBoxStartup.Location = new Point(388, 570);
            this.checkBoxStartup.Margin = new Padding(4, 5, 4, 5);
            this.checkBoxStartup.Name = "checkBoxStartup";
            this.checkBoxStartup.Size = new Size(272, 29);
            this.checkBoxStartup.TabIndex = 4;
            this.checkBoxStartup.Text = "Launch application on startup";
            this.checkBoxStartup.UseVisualStyleBackColor = true;
            this.checkBoxStartup.CheckedChanged += this.CheckBoxStartup_CheckedChanged;
            // 
            // BtnShowLoadProfiles
            // 
            this.BtnShowLoadProfiles.AccessibleName = "BtnShowLoadProfiles";
            this.BtnShowLoadProfiles.BackColor = Color.LightSkyBlue;
            this.BtnShowLoadProfiles.Location = new Point(388, 281);
            this.BtnShowLoadProfiles.Margin = new Padding(4, 5, 4, 5);
            this.BtnShowLoadProfiles.Name = "BtnShowLoadProfiles";
            this.BtnShowLoadProfiles.Size = new Size(286, 66);
            this.BtnShowLoadProfiles.TabIndex = 8;
            this.BtnShowLoadProfiles.Text = "Load Profiles";
            this.BtnShowLoadProfiles.UseVisualStyleBackColor = false;
            this.BtnShowLoadProfiles.Click += this.BtnShowLoadProfiles_Click;
            // 
            // BtnShowDeleteProfiles
            // 
            this.BtnShowDeleteProfiles.AccessibleName = "BtnShowDeleteProfiles";
            this.BtnShowDeleteProfiles.BackColor = Color.LightSkyBlue;
            this.BtnShowDeleteProfiles.Location = new Point(755, 281);
            this.BtnShowDeleteProfiles.Margin = new Padding(4, 5, 4, 5);
            this.BtnShowDeleteProfiles.Name = "BtnShowDeleteProfiles";
            this.BtnShowDeleteProfiles.Size = new Size(286, 66);
            this.BtnShowDeleteProfiles.TabIndex = 9;
            this.BtnShowDeleteProfiles.Text = "Delete Profile";
            this.BtnShowDeleteProfiles.UseVisualStyleBackColor = false;
            this.BtnShowDeleteProfiles.Click += this.BtnShowDeleteProfiles_Click;
            // 
            // BtnCycleAudioOutputs
            // 
            this.BtnCycleAudioOutputs.AccessibleName = "BtnCycleAudioOutputs";
            this.BtnCycleAudioOutputs.BackColor = Color.LightSkyBlue;
            this.BtnCycleAudioOutputs.Location = new Point(29, 402);
            this.BtnCycleAudioOutputs.Margin = new Padding(4, 5, 4, 5);
            this.BtnCycleAudioOutputs.Name = "BtnCycleAudioOutputs";
            this.BtnCycleAudioOutputs.Size = new Size(286, 66);
            this.BtnCycleAudioOutputs.TabIndex = 5;
            this.BtnCycleAudioOutputs.Text = "Cycle Audio Devices";
            this.BtnCycleAudioOutputs.UseVisualStyleBackColor = false;
            this.BtnCycleAudioOutputs.Click += this.BtnCycleAudioOutputs_Click;
            // 
            // BtnSetProfileHotkey
            // 
            this.BtnSetProfileHotkey.BackColor = Color.LightSkyBlue;
            this.BtnSetProfileHotkey.Location = new Point(755, 400);
            this.BtnSetProfileHotkey.Name = "BtnSetProfileHotkey";
            this.BtnSetProfileHotkey.Size = new Size(286, 66);
            this.BtnSetProfileHotkey.TabIndex = 7;
            this.BtnSetProfileHotkey.Text = "Set Profile Hotkey";
            this.BtnSetProfileHotkey.UseVisualStyleBackColor = false;
            this.BtnSetProfileHotkey.Click += this.BtnSetProfileHotkey_Click;
            // 
            // BtnSetAudioHotkey
            // 
            this.BtnSetAudioHotkey.BackColor = Color.LightSkyBlue;
            this.BtnSetAudioHotkey.Location = new Point(388, 401);
            this.BtnSetAudioHotkey.Name = "BtnSetAudioHotkey";
            this.BtnSetAudioHotkey.Size = new Size(286, 67);
            this.BtnSetAudioHotkey.TabIndex = 6;
            this.BtnSetAudioHotkey.Text = "Set Audio Hotkey";
            this.BtnSetAudioHotkey.UseVisualStyleBackColor = false;
            this.BtnSetAudioHotkey.Click += this.BtnSetAudioHotkey_Click;
            // 
            // Logo
            // 
            this.Logo.Location = new Point(42, 13);
            this.Logo.Margin = new Padding(4);
            this.Logo.Name = "Logo";
            this.Logo.Size = new Size(138, 128);
            this.Logo.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Logo.TabIndex = 8;
            this.Logo.TabStop = false;
            this.Logo.Click += this.Logo_Click;
            // 
            // LogoName
            // 
            this.LogoName.BackColor = Color.FromArgb(50, 50, 50);
            this.LogoName.BorderStyle = BorderStyle.None;
            this.LogoName.Font = new Font("Cascadia Code", 20F, FontStyle.Bold);
            this.LogoName.ForeColor = Color.White;
            this.LogoName.Location = new Point(204, 52);
            this.LogoName.Margin = new Padding(4);
            this.LogoName.Name = "LogoName";
            this.LogoName.ReadOnly = true;
            this.LogoName.Size = new Size(222, 47);
            this.LogoName.TabIndex = 9;
            this.LogoName.Text = "DAIRemote";
            // 
            // Description
            // 
            this.Description.BackColor = Color.FromArgb(50, 50, 50);
            this.Description.BorderStyle = BorderStyle.None;
            this.Description.ForeColor = Color.White;
            this.Description.Location = new Point(13, 149);
            this.Description.Margin = new Padding(4);
            this.Description.Multiline = true;
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Size = new Size(1106, 110);
            this.Description.TabIndex = 10;
            this.Description.Text = resources.GetString("Description.Text");
            // 
            // DAIRemoteApplicationUI
            // 
            this.AutoScaleDimensions = new SizeF(10F, 25F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(50, 50, 50);
            this.ClientSize = new Size(1110, 835);
            this.Controls.Add(this.BtnShowDeleteProfiles);
            this.Controls.Add(this.BtnShowLoadProfiles);
            this.Controls.Add(this.Description);
            this.Controls.Add(this.LogoName);
            this.Controls.Add(this.Logo);
            this.Controls.Add(this.BtnCycleAudioOutputs);
            this.Controls.Add(this.checkBoxStartup);
            this.Controls.Add(this.BtnSaveDisplayConfig);
            this.Controls.Add(this.BtnSetAudioHotkey);
            this.Controls.Add(this.BtnSetProfileHotkey);
            this.Margin = new Padding(4, 5, 4, 5);
            this.Name = "DAIRemoteApplicationUI";
            this.Text = "DAIRemote";
            Load += this.DAIRemoteApplicationUI_Load;
            ((System.ComponentModel.ISupportInitialize)this.Logo).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private PictureBox Logo;
        private TextBox LogoName;
        private TextBox Description;
        private Button BtnSaveDisplayConfig;
        private CheckBox checkBoxStartup;
        private Button BtnCycleAudioOutputs;
        private Button BtnSetAudioHotkey;
        private Button BtnSetProfileHotkey;
        private Button BtnShowDeleteProfiles;
        private Button BtnShowLoadProfiles;
    }
}