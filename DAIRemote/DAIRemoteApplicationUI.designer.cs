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
            this.BtnSaveDisplayConfig = new Button();
            this.checkBoxStartup = new CheckBox();
            this.BtnCycleAudioOutputs = new Button();
            this.DisplayLoadProfilesLayout = new FlowLayoutPanel();
            this.DisplayDeleteProfilesLayout = new FlowLayoutPanel();
            this.pictureBox1 = new PictureBox();
            this.textBox1 = new TextBox();
            this.textBox2 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
            this.SuspendLayout();
            // 
            // BtnSaveDisplayConfig
            // 
            this.BtnSaveDisplayConfig.AccessibleName = "BtnSaveDisplayConfig";
            this.BtnSaveDisplayConfig.BackColor = Color.LightSkyBlue;
            this.BtnSaveDisplayConfig.Location = new Point(23, 261);
            this.BtnSaveDisplayConfig.Margin = new Padding(3, 4, 3, 4);
            this.BtnSaveDisplayConfig.Name = "BtnSaveDisplayConfig";
            this.BtnSaveDisplayConfig.Size = new Size(229, 53);
            this.BtnSaveDisplayConfig.TabIndex = 0;
            this.BtnSaveDisplayConfig.Text = "Add Display Profile";
            this.BtnSaveDisplayConfig.UseVisualStyleBackColor = false;
            this.BtnSaveDisplayConfig.Click += this.BtnAddDisplayConfig_Click;
            // 
            // checkBoxStartup
            // 
            this.checkBoxStartup.AutoSize = true;
            this.checkBoxStartup.ForeColor = SystemColors.Control;
            this.checkBoxStartup.Location = new Point(23, 489);
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
            this.BtnCycleAudioOutputs.Location = new Point(23, 334);
            this.BtnCycleAudioOutputs.Margin = new Padding(3, 4, 3, 4);
            this.BtnCycleAudioOutputs.Name = "BtnCycleAudioOutputs";
            this.BtnCycleAudioOutputs.Size = new Size(229, 53);
            this.BtnCycleAudioOutputs.TabIndex = 5;
            this.BtnCycleAudioOutputs.Text = "Cycle Audio Devices";
            this.BtnCycleAudioOutputs.UseVisualStyleBackColor = false;
            this.BtnCycleAudioOutputs.Click += this.BtnCycleAudioOutputs_Click;
            // 
            // DisplayLoadProfilesLayout
            // 
            this.DisplayLoadProfilesLayout.Location = new Point(323, 16);
            this.DisplayLoadProfilesLayout.Margin = new Padding(3, 4, 3, 4);
            this.DisplayLoadProfilesLayout.Name = "DisplayLoadProfilesLayout";
            this.DisplayLoadProfilesLayout.Size = new Size(544, 237);
            this.DisplayLoadProfilesLayout.TabIndex = 6;
            // 
            // DisplayDeleteProfilesLayout
            // 
            this.DisplayDeleteProfilesLayout.Location = new Point(323, 261);
            this.DisplayDeleteProfilesLayout.Margin = new Padding(3, 4, 3, 4);
            this.DisplayDeleteProfilesLayout.Name = "DisplayDeleteProfilesLayout";
            this.DisplayDeleteProfilesLayout.Size = new Size(544, 252);
            this.DisplayDeleteProfilesLayout.TabIndex = 7;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(110, 110);
            this.pictureBox1.Image = Image.FromFile("Resources/DAIRemoteLogo.ico");
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = Color.FromArgb(50, 50, 50);
            this.textBox1.BorderStyle = BorderStyle.None;
            this.textBox1.Font = new Font("Cascadia Code", 20F, FontStyle.Bold);
            this.textBox1.ForeColor = Color.White;
            this.textBox1.Location = new Point(106, 31);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Size(211, 39);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "DAIRemote";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Point(1, 110);
            this.textBox2.Name = "textBox2";
            this.textBox2.BackColor = Color.FromArgb(50, 50, 50);
            this.textBox2.BorderStyle = BorderStyle.None;
            this.textBox2.ForeColor = Color.White;
            this.textBox2.Text = "Load profile: top. Delete profile: bottom. Enjoy using this program!";
            this.textBox2.Multiline = true;
            this.textBox2.Size = new Size(276, 75);
            this.textBox2.TabIndex = 10;
            // 
            // DAIRemoteApplicationUI
            // 
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(50, 50, 50);
            this.ClientSize = new Size(881, 527);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.DisplayDeleteProfilesLayout);
            this.Controls.Add(this.DisplayLoadProfilesLayout);
            this.Controls.Add(this.BtnCycleAudioOutputs);
            this.Controls.Add(this.checkBoxStartup);
            this.Controls.Add(this.BtnSaveDisplayConfig);
            this.Margin = new Padding(3, 4, 3, 4);
            this.Name = "DAIRemoteApplicationUI";
            this.Text = "DAIRemote";
            Load += this.DAIRemoteApplicationUI_Load;
            ((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Button BtnSaveDisplayConfig;
        private CheckBox checkBoxStartup;
        private Button BtnCycleAudioOutputs;
        private FlowLayoutPanel DisplayLoadProfilesLayout;
        private FlowLayoutPanel DisplayDeleteProfilesLayout;
        private PictureBox pictureBox1;
        private TextBox textBox1;
        private TextBox textBox2;
    }
}