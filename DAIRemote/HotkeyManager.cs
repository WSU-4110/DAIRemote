using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace DAIRemote
{
    public class HotkeyManager
    {
        public Keys SelectedHotkey { get; private set; }
        private Label lblCurrentHotkey;
        private AudioDeviceManager.AudioDeviceManager audioManager;
        internal const string HotkeyConfigFile = "hotkeys.json";  // Changed to internal for access

        public HotkeyManager(Label hotkeyLabel, AudioDeviceManager.AudioDeviceManager audioDeviceManager)
        {
            lblCurrentHotkey = hotkeyLabel;
            audioManager = audioDeviceManager;
            SelectedHotkey = Keys.None;
            LoadHotkeys();
        }

        public void SetHotkey(Keys hotkey)
        {
            SelectedHotkey = hotkey;
            lblCurrentHotkey.Text = $"Current Hotkey: {SelectedHotkey}";
            SaveHotkeys();
        }

        private void SaveHotkeys()
        {
            var existingHotkeys = LoadHotkeys();

            if (!existingHotkeys.Contains(SelectedHotkey.ToString()))
            {
                existingHotkeys.Add(SelectedHotkey.ToString());
                var hotkeyData = new HotkeyData { Hotkeys = existingHotkeys };
                var json = JsonSerializer.Serialize(hotkeyData);
                File.WriteAllText(HotkeyConfigFile, json);
            }
        }


        public List<string> LoadHotkeys()
        {
            if (File.Exists(HotkeyConfigFile))
            {
                var json = File.ReadAllText(HotkeyConfigFile);
                var hotkeyData = JsonSerializer.Deserialize<HotkeyData>(json);
                return hotkeyData?.Hotkeys ?? new List<string>();
            }
            return new List<string>();
        }
        private class HotkeyData
        {
            public List<string> Hotkeys { get; set; }
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
