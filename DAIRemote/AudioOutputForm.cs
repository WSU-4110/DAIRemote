using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NAudio.CoreAudioApi;

namespace DAIRemote
{
    public partial class AudioOutputForm : Form
    {
        private ComboBox audioDeviceComboBox; // Define the ComboBox

        public AudioOutputForm()
        {
            InitializeComponent();
            audioDeviceComboBox = new ComboBox(); // Initialize the ComboBox
            Controls.Add(audioDeviceComboBox); // Add ComboBox to the form
            LoadAudioDevices();
        }

        private void InitializeComponent()
        {
            this.audioDeviceComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout(); // Start the layout of the form

            
            this.audioDeviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; // Prevent user input
            this.audioDeviceComboBox.FormattingEnabled = true;
            this.audioDeviceComboBox.Location = new System.Drawing.Point(12, 12); // Set position
            this.audioDeviceComboBox.Name = "audioDeviceComboBox";
            this.audioDeviceComboBox.Size = new System.Drawing.Size(260, 21); // Set size
            this.audioDeviceComboBox.TabIndex = 0;

            this.ClientSize = new System.Drawing.Size(284, 61); // Set the form size
            this.Controls.Add(this.audioDeviceComboBox); // Add the ComboBox to the form
            this.Name = "AudioOutputForm";
            this.Text = "Audio Output Switcher"; // Set the title
            this.ResumeLayout(false); // End the layout of the form
        }

        private void LoadAudioDevices()
        {
            var devices = GetAudioDevices();
            audioDeviceComboBox.DataSource = devices;
        }

        public List<string> GetAudioDevices()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            return devices.Select(d => d.FriendlyName).ToList();
        }
    }
}
