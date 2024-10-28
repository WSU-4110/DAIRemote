using AudioSwitcher.AudioApi.CoreAudio;
using System.Diagnostics;

namespace AudioDeviceManager
{
    public class AudioDeviceManager
    {
        private CoreAudioDevice defaultAudioDevice;
        private CoreAudioController controller;
        private List<CoreAudioDevice> devices;

        public AudioDeviceManager()
        {
            controller = new CoreAudioController();
            defaultAudioDevice = controller.DefaultPlaybackDevice;
            devices = getAllActiveAudioDevices().ToList();
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
            this.defaultAudioDevice.Volume = volume;
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
        }
    }
}
