using System.Runtime.InteropServices;

namespace UDPServerManager;

public class SpecialKeys
{
    const int keyDown = 0x1;
    const int keyUp = 0x2;

    // Windows key
    public const byte LWin = 0x5B;

    // Media keys
    public const byte nextTrack = 0xB0;
    public const byte previousTrack = 0xB1;
    public const byte playPauseTrack = 0xB3;

    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    public static void PressKey(byte key)
    {
        // Press Windows key
        keybd_event(key, 0, keyDown, 0);

        // Release Windows key
        keybd_event(key, 0, keyUp, 0);

    }

    public static void ComboKeys(byte modifierKey, byte secondaryKey)
    {
        keybd_event(modifierKey, 0, keyDown, 0);

        keybd_event(secondaryKey, 0, keyDown, 0);
        keybd_event(secondaryKey, 0, keyUp, 0);

        keybd_event(modifierKey, 0, keyUp, 0);
    }

    public static void KeyDown(byte key)
    {
        // Hold key down
        keybd_event(LWin, 0, keyDown, 0);
    }

    public static void KeyUp(byte key)
    {
        // Release key
        keybd_event(LWin, 0, keyUp, 0);
    }
}