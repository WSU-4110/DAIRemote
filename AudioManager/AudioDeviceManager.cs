using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using System.Diagnostics;

namespace AudioDeviceManager
{
    public class AudioDeviceManager
    {
        private static AudioDeviceManager aduioDeviceManagerInstance;
        private CoreAudioDevice defaultAudioDevice;
        private CoreAudioController audioController;
        private List<CoreAudioDevice> audioDevices;
        private double defaultAudioDeviceVolume;
        private bool defaultAudioMuteStatus;

        public delegate void AudioDevicesUpdatedEventHandler(List<string> devices);
        public event AudioDevicesUpdatedEventHandler audioDevicesUpdated;

        public AudioDeviceManager()
        {
            // Initialize CoreAudioController, which combs through
            // all the available audio sources, hence the slow
            // initialization.
            audioController = new CoreAudioController();

            // Set the initial values for variables needed for functions
            SetAudioDefaults();

            // Subscribe all playback devices, even inactive ones
            foreach (CoreAudioDevice device in audioController.GetPlaybackDevices())
            {
                device.StateChanged.Subscribe(OnAudioDeviceChanged);
            }
        }

        private void SetAudioDefaults()
        {
            // Set the initial values for variables needed for functions
            defaultAudioDevice = audioController.DefaultPlaybackDevice;
            defaultAudioDeviceVolume = this.defaultAudioDevice.Volume;
            defaultAudioMuteStatus = defaultAudioDevice.IsMuted;

            // Get list of active devices
            SetActiveDevices();
        }

        private void OnAudioDeviceChanged(DeviceChangedArgs args)
        {
            Debug.WriteLine($"Audio device state changed for: {args.Device.FullName}");
            SetAudioDefaults();
            audioDevicesUpdated?.Invoke(ActiveDeviceNames);
        }

        public static AudioDeviceManager GetInstance()
        {
            if (aduioDeviceManagerInstance == null)
            {
                aduioDeviceManagerInstance = new AudioDeviceManager();
            }
            return aduioDeviceManagerInstance;
        }

        public CoreAudioDevice GetDefaultAudioDevice() { return this.defaultAudioDevice; }

        public void SetDefaultAudioDevice(CoreAudioDevice audioDevice)
        {
            this.defaultAudioDevice = audioDevice;
            this.defaultAudioDevice.SetAsDefault();
        }

        public void SetDefaultAudioDevice(string deviceFullName)
        {
            CoreAudioDevice matchedDevice = audioDevices.FirstOrDefault(d => d.FullName == deviceFullName);

            if (matchedDevice != null)
            {
                SetDefaultAudioDevice(matchedDevice);
            }
            else
            {
                throw new ArgumentException($"No device found with the name '{deviceFullName}'");
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

        public List<string> ActiveDeviceNames
        {
            get { return audioDevices.Select(d => d.FullName).ToList(); }
        }

        public void SetVolume(int volume)
        {
            defaultAudioDeviceVolume = volume;
            this.defaultAudioDevice.Volume = volume;
        }

        public void IncreaseVolume(int increment = 1)
        {
            double volume = GetVolume();
            if (volume < 100 && increment > 0)
            {
                double setVolume = volume + increment;
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

        public void DecreaseVolume(int decrement = 1)
        {
            double volume = GetVolume();
            if (volume > 0 && decrement > 0)
            {
                double setVolume = volume - decrement;
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

        public void CycleAudioDevice()
        {
            // Get the index of the current default device
            int currentIndex = audioDevices.FindIndex(device => device.Id == defaultAudioDevice.Id);
            // Calculate the next index in a circular manner
            int nextIndex = (currentIndex + 1) % audioDevices.Count;
            // Set the next device as the default audio device
            SetDefaultAudioDevice(audioDevices[nextIndex]);
            Debug.WriteLine($"Switched to {defaultAudioDevice.FullName}");
            audioDevicesUpdated?.Invoke(ActiveDeviceNames);
        }
    }
}
