using System;
using System.Windows.Forms;

namespace DAIRemote
{
    public partial class Form1
    {

        private void InitializeCustomComponents()
        {
            this.btnShowAudioOutputs = new Button();
            this.audioFormPanel = new Panel();
            this.SuspendLayout();

            // 
            // btnShowAudioOutputs
            // 
            this.btnShowAudioOutputs.Location = new System.Drawing.Point(10, 10); // Keep it in the top left corner
            this.btnShowAudioOutputs.Name = "btnShowAudioOutputs";
            this.btnShowAudioOutputs.Size = new System.Drawing.Size(150, 30);
            this.btnShowAudioOutputs.TabIndex = 0;
            this.btnShowAudioOutputs.Text = "Show Audio Outputs";
            this.btnShowAudioOutputs.UseVisualStyleBackColor = true;
            this.btnShowAudioOutputs.Click += BtnShowAudioOutputs_Click;

            // 
            // audioFormPanel
            // 
            this.audioFormPanel.Location = new System.Drawing.Point(10, 50); // Adjust this to avoid overlap
            this.audioFormPanel.Size = new System.Drawing.Size(300, 200); // Adjust size as needed
            this.audioFormPanel.BorderStyle = BorderStyle.FixedSingle; // Optional: add a border to visualize the panel

            // 
            // BtnSaveDisplayConfig
            // 
            BtnSaveDisplayConfig.AccessibleName = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.Location = new Point(180, 10); // Position it to the right of the audio button
            BtnSaveDisplayConfig.Name = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.Size = new Size(150, 30);
            BtnSaveDisplayConfig.TabIndex = 1; // Ensure tab index is sequential
            BtnSaveDisplayConfig.Text = "Save Display Config";
            BtnSaveDisplayConfig.UseVisualStyleBackColor = true;
            BtnSaveDisplayConfig.Click += BtnSaveDisplayConfig_Click;

            // 
            // Form1
            // 
            this.Controls.Add(this.btnShowAudioOutputs);
            this.Controls.Add(this.audioFormPanel);
            this.Controls.Add(BtnSaveDisplayConfig);
            this.Name = "Form1";
            this.Size = new System.Drawing.Size(320, 300); // Adjust size as needed
            this.ResumeLayout(false);
        }


    }
}
