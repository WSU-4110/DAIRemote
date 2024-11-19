using System.Text.Json.Serialization;

namespace DAIRemote;
public class HotkeyConfig
{
    public string Action { get; set; }
    public uint Modifiers { get; set; }
    public uint Key { get; set; }

    [JsonIgnore]
    public Action Callback { get; set; } // Function to execute
}
