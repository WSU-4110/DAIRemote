using System;
using System.Windows.Forms;

namespace DAIRemote
{
    public class HotkeyManager
    {
        public Keys SelectedHotkey { get; private set; }
        private Label lblCurrentHotkey;
        private AudioDeviceManager.AudioDeviceManager audioManager;

        public HotkeyManager(Label hotkeyLabel, AudioDeviceManager.AudioDeviceManager audioDeviceManager)
        {
            lblCurrentHotkey = hotkeyLabel;
            audioManager = audioDeviceManager;
            SelectedHotkey = Keys.None; 
        }

        public void SetHotkey(Keys hotkey)
        {
            SelectedHotkey = hotkey;
            lblCurrentHotkey.Text = $"Current Hotkey: {SelectedHotkey}";
        }

        public void HandleKeyPress(Keys keyData)
        {
            if (keyData == SelectedHotkey)
            {
                audioManager.CycleToNextAudioDevice();
            }
        }
    }
}
