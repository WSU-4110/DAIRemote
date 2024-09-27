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
            LoadAudioDevices();
        }

        private void InitializeComponent()
        {
            audioDeviceComboBox = new ComboBox();
            SuspendLayout();
            // 
            // audioDeviceComboBox
            // 
            audioDeviceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            audioDeviceComboBox.FormattingEnabled = true;
            audioDeviceComboBox.Location = new Point(12, 12);
            audioDeviceComboBox.Name = "audioDeviceComboBox";
            audioDeviceComboBox.Size = new Size(260, 33);
            audioDeviceComboBox.TabIndex = 0;

            audioDeviceComboBox.DropDown += audioDeviceComboBox_DropDown;
            audioDeviceComboBox.SelectedIndexChanged += audioDeviceComboBox_SelectedIndexChanged;
            // 
            // AudioOutputForm
            // 
            ClientSize = new Size(284, 61);
            Controls.Add(audioDeviceComboBox);
            Name = "AudioOutputForm";
            Text = "Audio Output Switcher";
            ResumeLayout(false);
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

        private void audioDeviceComboBox_DropDown(object sender, EventArgs e)
        {
            LoadAudioDevices();
        }
        private void audioDeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
