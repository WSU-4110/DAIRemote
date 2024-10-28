namespace AudioDeviceManager
{
    public partial class AudioOutputForm : Form
    {
        private ComboBox audioDeviceComboBox;
        private static AudioOutputForm instance;
        private AudioDeviceManager audioManager;
        private List<string> devices;
        public AudioOutputForm(AudioDeviceManager audioManager)
        {
            this.audioManager = audioManager;
            this.devices = audioManager.ActiveDeviceNames;
            audioDeviceComboBox = new ComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(12, 100),
                Name = "audioDeviceComboBox",
                Size = new Size(260, 33),
                TabIndex = 0
            };

            audioDeviceComboBox.SelectedIndexChanged += audioDeviceComboBox_SelectedIndexChanged;
            Controls.Add(audioDeviceComboBox);
            Name = "AudioOutputForm";
            Text = "Audio Output Switcher";
            BackColor = Color.FromArgb(50, 50, 50);
            InitializeAudioOutputFormComponent();
        }

        public static AudioOutputForm GetInstance(AudioDeviceManager audioManager)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new AudioOutputForm(audioManager);
            }
            return instance;
        }

        private void InitializeAudioOutputFormComponent()
        {
            audioDeviceComboBox.Items.Clear();
            string defaultDevice = audioManager.getDefaultAudioDevice().FullName;
            int defaultIndex = -1;

            for (int i = 0; i < devices.Count; i++)
            {
                audioDeviceComboBox.Items.Add(devices[i]);
                if (devices[i] == defaultDevice)
                {
                    defaultIndex = i;
                }
            }

            if (defaultIndex != -1)
            {
                audioDeviceComboBox.SelectedIndex = defaultIndex;
            }
        }

        private void audioDeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDevice = audioDeviceComboBox.SelectedItem as string;
            if (selectedDevice != null && selectedDevice != audioManager.getDefaultAudioDevice().FullName)
            {
                audioManager.setDefaultAudioDevice(selectedDevice);
            }
        }
    }
}
