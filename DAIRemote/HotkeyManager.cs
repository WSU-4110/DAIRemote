using System.Text.Json;

public class HotkeyManager
{
    public Keys SelectedHotkey { get; private set; }
    private Label lblCurrentHotkey;
    private AudioManager.AudioDeviceManager audioManager;
    private ComboBox hotkeyComboBox;

    internal const string HotkeyConfigFile = "hotkeys.json";
    private bool isUpdatingComboBoxSelection = false;

    public HotkeyManager(Label hotkeyLabel, AudioManager.AudioDeviceManager audioDeviceManager, ComboBox hotkeyComboBox)
    {
        lblCurrentHotkey = hotkeyLabel;
        audioManager = audioDeviceManager;
        this.hotkeyComboBox = hotkeyComboBox;
        SelectedHotkey = Keys.None;
        LoadHotkeys();
    }

    public void SetHotkey(Keys hotkey)
    {
        SelectedHotkey = hotkey;
        lblCurrentHotkey.Text = $"Current Hotkey: {SelectedHotkey}";

        var existingHotkeys = LoadHotkeys();

        if (!existingHotkeys.Contains(SelectedHotkey.ToString()))
        {
            existingHotkeys.Add(SelectedHotkey.ToString());
            SaveHotkeys(existingHotkeys);
        }

        if (!isUpdatingComboBoxSelection)
        {
            isUpdatingComboBoxSelection = true;
            UpdateHotkeyComboBox(existingHotkeys);
            hotkeyComboBox.SelectedItem = SelectedHotkey != Keys.None ? SelectedHotkey.ToString() : "None";
            isUpdatingComboBoxSelection = false;
        }
    }

    private void SaveHotkeys(List<string> hotkeys)
    {
        var hotkeyData = new HotkeyData { Hotkeys = hotkeys };
        var json = JsonSerializer.Serialize(hotkeyData);
        File.WriteAllText(HotkeyConfigFile, json);
    }

    private void UpdateHotkeyComboBox(List<string> existingHotkeys)
    {
        hotkeyComboBox.Items.Clear();
        hotkeyComboBox.Items.Add("None");

        foreach (var hotkey in existingHotkeys)
        {
            hotkeyComboBox.Items.Add(hotkey);
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
            audioManager.CycleAudioDevice();
        }
    }

    public void OnComboBoxSelectionChanged(object sender, EventArgs e)
    {
        if (isUpdatingComboBoxSelection)
        {
            return;
        }

        if (hotkeyComboBox.SelectedItem != null)
        {
            var selectedHotkey = hotkeyComboBox.SelectedItem.ToString();

            if (selectedHotkey != "None")
            {
                if (Enum.TryParse<Keys>(selectedHotkey, out Keys parsedHotkey))
                {
                    SetHotkey(parsedHotkey);
                }
            }
            else
            {
                SetHotkey(Keys.None);
            }
        }
    }
}
