using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using System.Diagnostics;

namespace AudioDeviceManager
{
    public class AudioDeviceManager
    {
        private static AudioDeviceManager instance;
        private CoreAudioDevice defaultAudioDevice;
        private CoreAudioController controller;
        private List<CoreAudioDevice> devices;
        private double defaultAudioDeviceVolume;
        private bool defaultAudioMuteStatus;

        public delegate void AudioDevicesUpdatedEventHandler(List<string> devices);
        public event AudioDevicesUpdatedEventHandler audioDevicesUpdated;

        public AudioDeviceManager()
        {
            // Initialize CoreAudioController, which combs through
            // all the available audio sources, hence the slow
            // initialization.
            controller = new CoreAudioController();

            // Set the initial values for variables needed for functions
            defaultAudioDevice = controller.DefaultPlaybackDevice;
            defaultAudioDeviceVolume = this.defaultAudioDevice.Volume;
            defaultAudioMuteStatus = defaultAudioDevice.IsMuted;

            // Get list of active devices
            setActiveDevices();

            // Subscribe all playback devices, even inactive ones
            foreach (CoreAudioDevice device in controller.GetPlaybackDevices())
            {
                device.StateChanged.Subscribe(OnAudioDeviceChanged);
            }
        }

        private void setAudioDefaults()
        {
            // Set the initial values for variables needed for functions
            defaultAudioDevice = controller.DefaultPlaybackDevice;
            defaultAudioDeviceVolume = this.defaultAudioDevice.Volume;
            defaultAudioMuteStatus = defaultAudioDevice.IsMuted;

            // Get list of active devices
            setActiveDevices();
        }

        private void OnAudioDeviceChanged(DeviceChangedArgs args)
        {
            Debug.WriteLine($"Audio device state changed for: {args.Device.FullName}");
            setAudioDefaults();
            audioDevicesUpdated?.Invoke(ActiveDeviceNames);
        }

        public static AudioDeviceManager GetInstance()
        {
            if (instance == null)
            {
                instance = new AudioDeviceManager();
            }
            return instance;
        }

        public CoreAudioDevice getDefaultAudioDevice() { return this.defaultAudioDevice; }

        public void setDefaultAudioDevice(CoreAudioDevice audioDevice)
        {
            this.defaultAudioDevice = audioDevice;
            this.defaultAudioDevice.SetAsDefault();
        }

        public void setDefaultAudioDevice(string deviceFullName)
        {
            CoreAudioDevice matchedDevice = devices.FirstOrDefault(d => d.FullName == deviceFullName);

            if (matchedDevice != null)
            {
                setDefaultAudioDevice(matchedDevice);
            }
            else
            {
                throw new ArgumentException($"No device found with the name '{deviceFullName}'");
            }
        }

        public List<CoreAudioDevice> getActiveDevices() { return this.devices; }

        public void setActiveDevices()
        {
            devices = getAllActiveAudioDevices().ToList();
        }

        public IEnumerable<CoreAudioDevice> getAllActiveAudioDevices()
        {
            return controller.GetPlaybackDevices().Where(device => device.State == AudioSwitcher.AudioApi.DeviceState.Active);
        }

        public List<string> ActiveDeviceNames
        {
            get { return devices.Select(d => d.FullName).ToList(); }
        }

        public void SetVolume(int volume)
        {
            defaultAudioDeviceVolume = volume;
            this.defaultAudioDevice.Volume = volume;
        }

        public void IncVolume(int increment = 1)
        {
            double vol = GetVolume();
            if (vol < 100 && increment > 0)
            {
                double setVolume = vol + increment;
                if (setVolume <= 100)
                {
                    SetVolume((int)setVolume);
                }
                else
                {
                    SetVolume(100);
                }
            }
        }

        public void DecVolume(int decrement = 1)
        {
            double vol = GetVolume();
            if (vol > 0 && decrement > 0)
            {
                double setVolume = vol - decrement;
                if (setVolume >= 0)
                {
                    SetVolume((int)setVolume);
                }
                else
                {
                    SetVolume(0);
                }
            }
        }

        public double GetVolume()
        {
            return defaultAudioDeviceVolume;
        }

        public void SetAudioMute(bool mute = true)
        {
            defaultAudioMuteStatus = mute;
            this.defaultAudioDevice.Mute(mute);
        }

        public void ToggleAudioMute()
        {
            SetAudioMute(!defaultAudioMuteStatus);
        }

        public void CycleToNextAudioDevice()
        {
            // Get the index of the current default device
            int currentIndex = devices.FindIndex(device => device.Id == defaultAudioDevice.Id);
            // Calculate the next index in a circular manner
            int nextIndex = (currentIndex + 1) % devices.Count;
            // Set the next device as the default audio device
            setDefaultAudioDevice(devices[nextIndex]);
            Debug.WriteLine($"Switched to {defaultAudioDevice.FullName}");
            audioDevicesUpdated?.Invoke(ActiveDeviceNames);
        }
    }
}
