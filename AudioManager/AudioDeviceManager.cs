using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;

namespace AudioManager;

public class AudioDeviceManager
{
    private static AudioDeviceManager aduioDeviceManagerInstance;
    private CoreAudioDevice defaultAudioDevice;
    private CoreAudioController audioController;
    private List<CoreAudioDevice> audioDevices;
    private IDisposable defaultDeviceSubscription;
    private List<IDisposable> deviceSubscriptions = new List<IDisposable>();

    public delegate void AudioDevicesUpdatedEventHandler(List<string> devices);
    public event AudioDevicesUpdatedEventHandler AudioDevicesUpdated;

    public AudioDeviceManager()
    {
        // Initialize CoreAudioController, which combs through
        // all the available audio sources, hence the slow
        // initialization.
        audioController = new CoreAudioController();

        // Subscribe to default device changes
        defaultDeviceSubscription = audioController.AudioDeviceChanged.Subscribe(OnDefaultDeviceChanged);

        // Set the initial values for variables needed for functions
        SetAudioDefaults();
        SubscribeAudioDevices();
    }

    private void OnDefaultDeviceChanged(DeviceChangedArgs args)
    {
        if (args.ChangedType == DeviceChangedType.DefaultChanged)
        {
            SetAudioDefaults();
            AudioDevicesUpdated?.Invoke(ActiveDeviceNames);
        }
    }

    private void OnAudioDeviceChanged(DeviceChangedArgs args)
    {
        SetAudioDefaults();
        AudioDevicesUpdated?.Invoke(ActiveDeviceNames);
    }

    public void SubscribeAudioDevices()
    {
        // Clear existing subscriptions
        foreach (var sub in deviceSubscriptions)
        {
            sub.Dispose();
        }
        deviceSubscriptions.Clear();

        // Subscribe to all playback devices
        foreach (CoreAudioDevice device in audioController.GetPlaybackDevices())
        {
            var subscription = device.StateChanged.Subscribe(OnAudioDeviceChanged);
            deviceSubscriptions.Add(subscription);
        }
    }

    private void SetAudioDefaults()
    {
        // Set the initial values for variables needed for functions
        defaultAudioDevice = audioController.DefaultPlaybackDevice;
        // Get list of active devices
        SetActiveDevices();
    }

    public void RefreshAudioDeviceSubscriptions()
    {
        audioController?.Dispose();
        audioController = new CoreAudioController();

        // Renew default device subscription
        defaultDeviceSubscription?.Dispose();
        defaultDeviceSubscription = audioController.AudioDeviceChanged.Subscribe(OnDefaultDeviceChanged);

        SetAudioDefaults();
        SubscribeAudioDevices();
        AudioDevicesUpdated?.Invoke(ActiveDeviceNames);
    }

    public void Dispose()
    {
        defaultDeviceSubscription?.Dispose();
        foreach (var sub in deviceSubscriptions)
        {
            sub.Dispose();
        }
        audioController?.Dispose();
    }

    public static AudioDeviceManager GetInstance()
    {
        aduioDeviceManagerInstance ??= new AudioDeviceManager();
        return aduioDeviceManagerInstance;
    }

    public CoreAudioDevice GetDefaultAudioDevice() { return this.defaultAudioDevice; }

    public void SetDefaultAudioDevice(CoreAudioDevice audioDevice)
    {
        if (audioDevice != this.defaultAudioDevice)
        {
            this.defaultAudioDevice = audioDevice;
            this.defaultAudioDevice.SetAsDefault();
        }
    }

    public void SetDefaultAudioDevice(string deviceFullName)
    {
        CoreAudioDevice matchedDevice = audioDevices.FirstOrDefault(d => d.FullName == deviceFullName);

        if (matchedDevice != null)
        {
            if (matchedDevice != this.defaultAudioDevice)
            {
                SetDefaultAudioDevice(matchedDevice);
            }
        }
    }

    public List<CoreAudioDevice> GetActiveDevices() { return this.audioDevices; }

    public void SetActiveDevices()
    {
        audioDevices = GetAllActiveAudioDevices().ToList();
    }

    public IEnumerable<CoreAudioDevice> GetAllActiveAudioDevices()
    {
        return audioController.GetPlaybackDevices().Where(device => device.State == AudioSwitcher.AudioApi.DeviceState.Active);
    }

    public List<string> ActiveDeviceNames => audioDevices.Select(d => d.FullName).ToList();

    public void SetVolume(double volume)
    {
        this.defaultAudioDevice.Volume = volume;
    }

    public double GetVolume()
    {
        return this.defaultAudioDevice.Volume;
    }

    public void IncreaseVolume(int increment = 1)
    {
        double volume = GetVolume();
        if (volume < 100 && increment > 0)
        {
            double setVolume = volume + increment;
            if (setVolume <= 100)
            {
                SetVolume(setVolume);
            }
            else
            {
                SetVolume(100);
            }
        }
    }

    public void DecreaseVolume(int decrement = 1)
    {
        double volume = GetVolume();
        if (volume > 0 && decrement > 0)
        {
            double setVolume = volume - decrement;
            if (setVolume >= 0)
            {
                SetVolume(setVolume);
            }
            else
            {
                SetVolume(0);
            }
        }
    }

    public void SetAudioMute(bool mute = true)
    {
        this.defaultAudioDevice.Mute(mute);
    }

    public void ToggleAudioMute()
    {
        SetAudioMute(!this.defaultAudioDevice.IsMuted);
    }

    public void CycleAudioDevice()
    {
        if (this.audioDevices.Count > 1)
        {
            // Get the index of the current default device
            int currentIndex = audioDevices.FindIndex(device => device.Id == defaultAudioDevice.Id);
            // Calculate the next index in a circular manner
            int nextIndex = (currentIndex + 1) % audioDevices.Count;
            // Set the next device as the default audio device
            SetDefaultAudioDevice(audioDevices[nextIndex]);
            AudioDevicesUpdated?.Invoke(ActiveDeviceNames);
        }
    }
}
