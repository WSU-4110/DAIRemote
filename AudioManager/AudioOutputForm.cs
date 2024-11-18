namespace AudioManager;

public partial class AudioOutputForm : Form
{
    private ComboBox audioDevicesDropDown;
    private static AudioOutputForm audioFormInstance;
    private AudioDeviceManager audioManager;
    public AudioOutputForm(AudioDeviceManager audioManager)
    {
        this.audioManager = audioManager;
        audioDevicesDropDown = new ComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            FormattingEnabled = true,
            Location = new Point(29, 150),
            Name = "audioDeviceComboBox",
            Size = new Size(260, 33),
            TabIndex = 0
        };

        audioDevicesDropDown.SelectedIndexChanged += DropDownOnSelectedChange;
        Controls.Add(audioDevicesDropDown);
        Name = "AudioOutputForm";
        Text = "Audio Output Switcher";
        BackColor = Color.FromArgb(50, 50, 50);

        // Listen to AudioDeviceManager's event handler for notifications
        audioManager.AudioDevicesUpdated += OnAudioDevicesUpdated;
        PopulateDropDown(audioManager.ActiveDeviceNames);
    }

    public static AudioOutputForm GetInstance(AudioDeviceManager audioManager)
    {
        if (audioFormInstance == null || audioFormInstance.IsDisposed)
        {
            audioFormInstance = new AudioOutputForm(audioManager)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
        }
        return audioFormInstance;
    }

    private void OnAudioDevicesUpdated(List<string> devices)
    {
        if (InvokeRequired)
        {
            Invoke(new Action(() => PopulateDropDown(audioManager.ActiveDeviceNames)));
        }
        else
        {
            PopulateDropDown(audioManager.ActiveDeviceNames);
        }
    }

    private void PopulateDropDown(List<string> audioDevices)
    {
        audioDevicesDropDown.Items.Clear();
        string defaultAudioDevice = audioManager.GetDefaultAudioDevice().FullName;
        int defaultIndex = -1;

        for (int i = 0; i < audioDevices.Count; i++)
        {
            audioDevicesDropDown.Items.Add(audioDevices[i]);
            if (audioDevices[i] == defaultAudioDevice)
            {
                defaultIndex = i;
            }
        }

        if (defaultIndex != -1)
        {
            audioDevicesDropDown.SelectedIndex = defaultIndex;
        }
    }

    private void DropDownOnSelectedChange(object sender, EventArgs e)
    {
        if (audioDevicesDropDown.SelectedItem is string selectedDevice && selectedDevice != audioManager.GetDefaultAudioDevice().FullName)
        {
            audioManager.SetDefaultAudioDevice(selectedDevice);
        }
    }
}
