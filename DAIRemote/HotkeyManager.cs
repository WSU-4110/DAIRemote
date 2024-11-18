using DisplayProfileManager;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace DAIRemote;
public partial class HotkeyManager : Form
{
    public Dictionary<string, HotkeyConfig> hotkeyConfigs;
    private readonly string ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/hotkeys.json");

    public HotkeyManager()
    {
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
    private const uint MOD_WIN = 0x0008;

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
            InitializeHotkeys();
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
            RegisterHotKey(this.Handle, config.Action.GetHashCode(), config.Modifiers, config.Key);
        }
    }

    public void UnregisterHotkeys()
    {
        foreach (var config in hotkeyConfigs.Values)
        {
            UnregisterHotKey(this.Handle, config.Action.GetHashCode());
        }
    }

    public void RegisterCallbacks()
    {
        // Register Display Callbacks
        string[] displayProfiles = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles"), "*.json");

        foreach (string profile in displayProfiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(profile);
            if (hotkeyConfigs.ContainsKey(fileName))
            {
                hotkeyConfigs[fileName].Callback = () => DisplayConfig.SetDisplaySettings(profile);
            }
        }

        // Register Audio Cycling Callback
        if (hotkeyConfigs.ContainsKey("Audio Cycling"))
        {
            hotkeyConfigs["Audio Cycling"].Callback = AudioManager.AudioDeviceManager.GetInstance().CycleAudioDevice;
        }
    }

    public void ShowHotkeyInput(string action, Action functionAction)
    {
        Form inputForm = new()
        {
            Text = $"Set Hotkey for {action}",
            Size = new Size(300, 150),
            StartPosition = FormStartPosition.CenterScreen
        };

        TextBox inputBox = new()
        {
            Dock = DockStyle.Top,
            ReadOnly = true     // Prevents Manual input
        };

        Button okButton = new()
        {
            Text = "OK",
            Dock = DockStyle.Bottom,
            DialogResult = DialogResult.OK
        };

        Button clearButton = new()
        {
            Text = "Clear",
            Dock = DockStyle.Bottom
        };

        Button cancelButton = new()
        {
            Text = "Cancel",
            Dock = DockStyle.Bottom,
            DialogResult = DialogResult.Cancel
        };

        Panel buttonPanel = new()
        {
            Dock = DockStyle.Bottom
        };
        buttonPanel.Controls.Add(clearButton);
        buttonPanel.Controls.Add(okButton);
        buttonPanel.Controls.Add(cancelButton);

        uint modifiers = 0;
        uint keyCode = 0;

        clearButton.Click += (s, e) =>
        {
            inputBox.Text = "Cleared";
            modifiers = 0;
            keyCode = 0;

            UnregisterHotkey(action);
        };

        inputBox.KeyDown += (s, args) =>
        {
            modifiers = 0;
            if (args.Control) modifiers |= MOD_CONTROL;
            if (args.Alt) modifiers |= MOD_ALT;
            if (args.Shift) modifiers |= MOD_SHIFT;
            keyCode = (uint)args.KeyCode;

            // Display the combination in the input box
            inputBox.Text = $"{(args.Control ? "Ctrl+" : "")}{(args.Alt ? "Alt+" : "")}{(args.Shift ? "Shift+" : "")}{args.KeyCode}";
        };

        inputForm.Controls.Add(inputBox);
        inputForm.Controls.Add(buttonPanel);

        DialogResult result = inputForm.ShowDialog();

        if (result == DialogResult.OK && keyCode != 0)
        {
            HotkeyConfig config = new()
            {
                Action = action,
                Modifiers = modifiers,
                Key = keyCode,
                Callback = functionAction
            };

            RegisterNewHotkey(action, config);
        }
        else if (result == DialogResult.Cancel)
        {
            inputForm.Close();
        }
    }

    private void RegisterNewHotkey(string action, HotkeyConfig newConfig)
    {
        UnregisterHotkey(action);
        hotkeyConfigs[action] = newConfig;
        SaveHotkeyConfigs();
        RegisterHotKey(this.Handle, action.GetHashCode(), newConfig.Modifiers, newConfig.Key);
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

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        UnregisterHotkeys();         // Unregister hotkeys on application close
        base.OnFormClosing(e);
    }
}
