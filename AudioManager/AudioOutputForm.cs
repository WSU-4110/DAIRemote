namespace AudioDeviceManager
{
    public partial class AudioOutputForm : Form
    {
        private ComboBox audioDeviceComboBox;
        private static AudioOutputForm instance;
        private AudioDeviceManager audioManager;
        public AudioOutputForm(AudioDeviceManager audioManager)
        {
            this.audioManager = audioManager;
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

            // Listen to AudioDeviceManager's event handler for notifications
            audioManager.audioDevicesUpdated += OnAudioDevicesUpdated;
            InitializeAudioOutputFormComponent(audioManager.ActiveDeviceNames);
        }

        public static AudioOutputForm GetInstance(AudioDeviceManager audioManager)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new AudioOutputForm(audioManager)
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }
            return instance;
        }

        private void OnAudioDevicesUpdated(List<string> devices)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => InitializeAudioOutputFormComponent(audioManager.ActiveDeviceNames)));
            }
            else
            {
                InitializeAudioOutputFormComponent(audioManager.ActiveDeviceNames);
            }
        }

        private void InitializeAudioOutputFormComponent(List<string> devices)
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
