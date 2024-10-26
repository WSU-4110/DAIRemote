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

            InitializeAudioOutputFormComponent();
            LoadAudioDevices();
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
            SuspendLayout();

            audioDeviceComboBox = new ComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(12, 100),
                Name = "audioDeviceComboBox",
                Size = new Size(260, 33),
                TabIndex = 0,
            };

            audioDeviceComboBox.DropDown += audioDeviceComboBox_DropDown;
            audioDeviceComboBox.SelectedIndexChanged += audioDeviceComboBox_SelectedIndexChanged;

            ClientSize = new Size(284, 100);
            Controls.Add(audioDeviceComboBox);
            Name = "AudioOutputForm";
            Text = "Audio Output Switcher";
            Load += AudioOutputForm_Load;
            ResumeLayout(false);
        }

        private void LoadAudioDevices()
        {
            audioDeviceComboBox.DataSource = null;
            audioDeviceComboBox.DataSource = this.devices;
        }

        private void audioDeviceComboBox_DropDown(object sender, EventArgs e)
        {
            LoadAudioDevices();
        }

        private void audioDeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDevice = audioDeviceComboBox.SelectedItem as string;
            if (selectedDevice != null)
            {
                audioManager.setDefaultAudioDevice(selectedDevice);
            }
        }

        private void AudioOutputForm_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(50, 50, 50);
        }

    }
}
