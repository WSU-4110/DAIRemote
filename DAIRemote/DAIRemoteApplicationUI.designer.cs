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
            components = new System.ComponentModel.Container();
            BtnSaveDisplayConfig = new Button();
            BtnShowAudioOutputs = new Button();
            button1 = new Button();
            profileNameTextBox = new TextBox();
            button2 = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
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
            // button1
            // 
            button1.AccessibleName = "BtnLoadDisplayConfig";
            button1.BackColor = Color.LightSkyBlue;
            button1.Location = new Point(23, 531);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(229, 53);
            button1.TabIndex = 2;
            button1.Text = "Load Display Config";
            button1.UseVisualStyleBackColor = false;
            button1.Click += BtnLoadDisplayConfig_Click;
            // 
            // profileNameTextBox
            // 
            profileNameTextBox.Location = new Point(23, 16);
            profileNameTextBox.Margin = new Padding(3, 4, 3, 4);
            profileNameTextBox.Name = "profileNameTextBox";
            profileNameTextBox.Size = new Size(228, 27);
            profileNameTextBox.TabIndex = 3;
            // 
            // button2
            // 
            button2.AccessibleName = "BtnDeleteDisplayConfig";
            button2.BackColor = Color.LightSkyBlue;
            button2.Location = new Point(23, 470);
            button2.Margin = new Padding(3, 4, 3, 4);
            button2.Name = "button2";
            button2.Size = new Size(229, 53);
            button2.TabIndex = 4;
            button2.Text = "Delete Display Config";
            button2.UseVisualStyleBackColor = false;
            button2.Click += BtnDeleteDisplayConfig_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(211, 32);
            // 
            // DAIRemoteApplicationUI
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(225, 246, 225);
            ClientSize = new Size(343, 600);
            Controls.Add(button2);
            Controls.Add(profileNameTextBox);
            Controls.Add(button1);
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
        private Button button1;
        private TextBox profileNameTextBox;
        private Button button2;
        private ContextMenuStrip contextMenuStrip1;
    }
}