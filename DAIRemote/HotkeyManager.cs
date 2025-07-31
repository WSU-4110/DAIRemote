using DisplayProfileManager;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace DAIRemote;
public partial class HotkeyManager : Form
{
    private readonly AudioManager.AudioDeviceManager audioManager = AudioManager.AudioDeviceManager.GetInstance();
    public Dictionary<string, HotkeyConfig> hotkeyConfigs;
    private readonly string ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/hotkeys.json");
    private uint modifiers = 0;
    private uint keyCode = 0;
    private string action;

    public HotkeyManager()
    {
        InitializeComponent();
        LoadHotkeyConfigs();
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private void UnregisterHotkey(string action)
    {
        if (hotkeyConfigs.TryGetValue(action, out HotkeyConfig? value))
        {
            UnregisterHotKey(this.Handle, value.Action.GetHashCode());
            hotkeyConfigs.Remove(action);
            SaveHotkeyConfigs();
        }
    }

    // Modifier keys
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;

    public string GetHotkeyText(HotkeyConfig config)
    {
        List<string> keys = new List<string>();
        if ((config.Modifiers & MOD_CONTROL) != 0) keys.Add("CTRL");
        if ((config.Modifiers & MOD_ALT) != 0) keys.Add("ALT");
        if ((config.Modifiers & MOD_SHIFT) != 0) keys.Add("SHIFT");
        keys.Add(((Keys)config.Key).ToString());
        return string.Join(" + ", keys);
    }

    public void LoadHotkeyConfigs()
    {
        if (File.Exists(ConfigFilePath))
        {
            hotkeyConfigs = JsonConvert.DeserializeObject<Dictionary<string, HotkeyConfig>>(File.ReadAllText(ConfigFilePath));
        }
        else
        {
            hotkeyConfigs = new Dictionary<string, HotkeyConfig>();
        }
    }

    private void SaveHotkeyConfigs()
    {
        File.WriteAllText(ConfigFilePath, System.Text.Json.JsonSerializer.Serialize(hotkeyConfigs));
    }

    public void InitializeHotkeys()
    {
        RegisterHotkeys();
        RegisterCallbacks();
    }

    public void RegisterHotkeys()
    {
        foreach (var config in hotkeyConfigs.Values)
        {
            _ = RegisterHotKey(this.Handle, config.Action.GetHashCode(), config.Modifiers, config.Key);
        }
    }

    public void UnregisterHotkeys()
    {
        foreach (var config in hotkeyConfigs.Values)
        {
            _ = UnregisterHotKey(this.Handle, config.Action.GetHashCode());
        }
    }

    public void RegisterCallbacks()
    {
        // Register Display Callbacks
        foreach (string profile in DisplayConfig.GetDisplayProfiles())
        {
            string fileName = Path.GetFileNameWithoutExtension(profile);
            if (hotkeyConfigs.TryGetValue(fileName, out HotkeyConfig? display))
            {
                display.Callback = () => DisplayConfig.SetDisplaySettings(profile);
            }
        }

        // Register Audio Cycling Callback
        if (hotkeyConfigs.TryGetValue("Audio Cycling", out HotkeyConfig? audioCycling))
        {
            audioCycling.Callback = audioManager.CycleAudioDevice;
        }

        // Register Audio Callbacks
        foreach (string audioDeviceName in audioManager.ActiveDeviceNames)
        {
            if (hotkeyConfigs.TryGetValue(audioDeviceName, out HotkeyConfig? audioDevices))
            {
                audioDevices.Callback = () => audioManager.SetDefaultAudioDevice(audioDeviceName);
            }
        }
    }

    public void ShowHotkeyInput(string action, Action functionAction)
    {
        HotkeyManager HotkeyInputForm = new()
        {
            action = action,
            Text = $"Set Hotkey for {action}"
        };

        if (hotkeyConfigs.ContainsKey(action))
        {
            HotkeyInputForm.HotkeyInputBox.Text = GetHotkeyText(hotkeyConfigs[action]);
        }

        HotkeyInputForm.HotkeyInputBox.KeyDown += (s, e) =>
        {
            HotkeyInputForm.modifiers = 0;
            if (e.Control) HotkeyInputForm.modifiers |= MOD_CONTROL;
            if (e.Alt) HotkeyInputForm.modifiers |= MOD_ALT;
            if (e.Shift) HotkeyInputForm.modifiers |= MOD_SHIFT;
            HotkeyInputForm.keyCode = (uint)e.KeyCode;

            // Display the combination in the input box
            HotkeyInputForm.HotkeyInputBox.Text = $"{(e.Control ? "Ctrl+" : "")}{(e.Alt ? "Alt+" : "")}{(e.Shift ? "Shift+" : "")}{e.KeyCode}";
        };

        HotkeyInputForm.HotkeyFormOkBtn.Click += (sender, e) =>
        {
            if (HotkeyInputForm.keyCode != 0)
            {
                HotkeyConfig config = new()
                {
                    Action = action,
                    Modifiers = HotkeyInputForm.modifiers,
                    Key = HotkeyInputForm.keyCode,
                    Callback = functionAction
                };

                RegisterNewHotkey(action, config);
            }
        };

        HotkeyInputForm.HotkeyFormClearBtn.Click += (sender, e) =>
        {
            HotkeyInputForm.HotkeyInputBox.Text = "Cleared";
            HotkeyInputForm.modifiers = 0;
            HotkeyInputForm.keyCode = 0;

            UnregisterHotkey(action);
        };

        HotkeyInputForm.HotkeyFormCancelBtn.Click += (sender, e) =>
        {
            HotkeyInputForm.Close();
        };

        // Set the OK button as the action for Enter key
        HotkeyInputForm.AcceptButton = HotkeyInputForm.HotkeyFormOkBtn;

        // Set the Cancel button as the action for Esc key
        HotkeyInputForm.CancelButton = HotkeyInputForm.HotkeyFormCancelBtn;

        _ = HotkeyInputForm.ShowDialog();
    }

    private void RegisterNewHotkey(string action, HotkeyConfig newConfig)
    {
        UnregisterHotkey(action);
        hotkeyConfigs[action] = newConfig;
        SaveHotkeyConfigs();
        _ = RegisterHotKey(this.Handle, action.GetHashCode(), newConfig.Modifiers, newConfig.Key);
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_HOTKEY = 0x0312;
        if (m.Msg == WM_HOTKEY)
        {
            int id = m.WParam.ToInt32();
            var hotkey = hotkeyConfigs.Values.FirstOrDefault(h => h.Action.GetHashCode() == id);

            if (hotkey?.Callback != null)
            {
                hotkey.Callback();          // Execute the function
            }
        }
        base.WndProc(ref m);
    }
}
