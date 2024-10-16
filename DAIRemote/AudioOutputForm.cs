using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NAudio.CoreAudioApi;

namespace DAIRemote
{
    public interface IAudioDeviceFactory
    {
        List<string> GetAudioDevices();
    }

    public class NAudioDeviceFactory : IAudioDeviceFactory
    {
        public List<string> GetAudioDevices()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            return devices.Select(d => d.FriendlyName).ToList();
        }
    }

    public partial class AudioOutputForm : Form
    {
        private ComboBox audioDeviceComboBox;
        private IAudioDeviceFactory audioDeviceFactory;

        // Inject the factory through the constructor
        public AudioOutputForm(IAudioDeviceFactory factory)
        {
            this.audioDeviceFactory = factory;
            InitializeComponent();
            LoadAudioDevices();
        }

        private void InitializeComponent()
        {
            audioDeviceComboBox = new ComboBox();
            SuspendLayout();

            audioDeviceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            audioDeviceComboBox.FormattingEnabled = true;
            audioDeviceComboBox.Location = new System.Drawing.Point(12, 250);
            audioDeviceComboBox.Name = "audioDeviceComboBox";
            audioDeviceComboBox.Size = new System.Drawing.Size(260, 33);
            audioDeviceComboBox.BackColor = System.Drawing.Color.Gray;
            audioDeviceComboBox.TabIndex = 0;
            audioDeviceComboBox.DropDown += audioDeviceComboBox_DropDown;

            ClientSize = new System.Drawing.Size(284, 100);
            Controls.Add(audioDeviceComboBox);
            Name = "AudioOutputForm";
            Text = "Audio Output Switcher";
            Load += AudioOutputForm_Load;
            ResumeLayout(false);
        }

        private void AudioOutputForm_Load(object sender, EventArgs e)
        {
            LoadAudioDevices();
        }

        // Use the factory to load available audio devices into the ComboBox
        private void LoadAudioDevices()
        {
            var devices = audioDeviceFactory.GetAudioDevices();
            audioDeviceComboBox.DataSource = devices;
        }

        // Refresh device list when dropdown is opened
        private void audioDeviceComboBox_DropDown(object sender, EventArgs e)
        {
            LoadAudioDevices();
        }
    }

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create a factory and pass it to the form
            IAudioDeviceFactory audioFactory = new NAudioDeviceFactory();
            Application.Run(new AudioOutputForm(audioFactory));
        }
    }
}
