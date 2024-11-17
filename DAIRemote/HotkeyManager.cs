using DisplayProfileManager;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace DAIRemote;
public partial class HotkeyManager : Form
{
    public Dictionary<string, HotkeyConfig> hotkeyConfigs;
    private readonly string ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/hotkeys.json");

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    // Modifier keys
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint MOD_WIN = 0x0008;

    private ToolStripMenuItem CreateMenuItem(string action, EventHandler onClick)
    {
        string hotkeyText = hotkeyConfigs.ContainsKey(action) ? $" ({GetHotkeyText(hotkeyConfigs[action])})" : "";
        return new ToolStripMenuItem($"{action}{hotkeyText}", Image.FromFile("Resources/Monitor.ico"), onClick);
    }

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
        Form inputForm = new Form();
        inputForm.Text = $"Set Hotkey for {action}";
        inputForm.Size = new Size(300, 100);
        inputForm.StartPosition = FormStartPosition.CenterScreen;

        TextBox inputBox = new TextBox()
        {
            Dock = DockStyle.Top,
            ReadOnly = true     // Prevents Manual input
        };

        Button okButton = new Button
        {
            Text = "OK",
            Dock = DockStyle.Bottom,
            DialogResult = DialogResult.OK
        };

        Button cancelButton = new Button
        {
            Text = "Cancel",
            Dock = DockStyle.Bottom,
            DialogResult = DialogResult.Cancel
        };

        Panel buttonPanel = new Panel { Dock = DockStyle.Bottom };
        buttonPanel.Controls.Add(okButton);
        buttonPanel.Controls.Add(cancelButton);

        uint modifiers = 0;
        uint keyCode = 0;

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

        if (inputForm.ShowDialog() == DialogResult.OK && keyCode != 0)
        {
            HotkeyConfig config = new HotkeyConfig
            {
                Action = action,
                Modifiers = modifiers,
                Key = keyCode,
                Callback = functionAction
            };

            OnHotkeySelected(action, config, functionAction);
        }
    }

    private bool HotkeyExists(HotkeyConfig newConfig)
    {
        // Check if the hotkey already exists in the hotkeyConfigs
        foreach (var config in hotkeyConfigs.Values)
        {
            if (config.Modifiers == newConfig.Modifiers && config.Key == newConfig.Key && config.Action != newConfig.Action)
            {
                return true;
            }
        }
        return false;
    }

    private void OnHotkeySelected(string action, HotkeyConfig newConfig, Action functionAction)
    {
        // Check if hotkey already assigned, if so ask for confirmation to reassign
        if (HotkeyExists(newConfig))
        {
            var result = MessageBox.Show("This hotkey is already assigned to another action. Do you want to reassign it?",
                "Hotkey Conflict", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Unregister the old hotkey
                var oldAction = hotkeyConfigs.FirstOrDefault(kvp => kvp.Value.Equals(newConfig)).Key;
                if (oldAction != null)
                {
                    UnregisterHotKey(this.Handle, oldAction.GetHashCode());
                    hotkeyConfigs.Remove(oldAction);
                }

                // Register the new hotkey
                RegisterNewHotkey(action, newConfig);
            }
            else
            {
                // Allow the user to pick a different hotkey
                PromptForNewHotkey(action, functionAction);
            }
        }
        else
        {
            // Register the new hotkey
            RegisterNewHotkey(action, newConfig);
        }
    }

    private void RegisterNewHotkey(string action, HotkeyConfig newConfig)
    {
        if (hotkeyConfigs.ContainsKey(action))
        {
            UnregisterHotKey(this.Handle, hotkeyConfigs[action].Action.GetHashCode());
        }
        hotkeyConfigs[action] = newConfig;
        SaveHotkeyConfigs();
        RegisterHotKey(this.Handle, action.GetHashCode(), newConfig.Modifiers, newConfig.Key);
    }

    private void PromptForNewHotkey(string action, Action functionAction)
    {
        // Open the hotkey input dialog again so the user can choose a different hotkey
        ShowHotkeyInput(action, functionAction);
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
    {// Unregister hotkeys on application close
        foreach (var config in hotkeyConfigs.Values)
        {
            UnregisterHotKey(this.Handle, config.Action.GetHashCode());
        }
        base.OnFormClosing(e);
    }
}
