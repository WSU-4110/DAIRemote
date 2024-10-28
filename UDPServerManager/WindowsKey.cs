using System.Runtime.InteropServices;

namespace UDPServerManager
{
    public class WindowsKey
    {
        const int keyDown = 0x1;
        const int keyUp = 0x2;
        const byte LWin = 0x5B;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void PressWinKey()
        {
            // Press Windows key
            keybd_event(LWin, 0, keyDown, 0);

            // Release Windows key
            keybd_event(LWin, 0, keyUp, 0);

        }

        public static void ComboWinKey(byte key)
        {
            keybd_event(LWin, 0, keyDown, 0);

            keybd_event(key, 0, keyDown, 0);
            keybd_event(key, 0, keyUp, 0);

            keybd_event(LWin, 0, keyUp, 0);
        }

        public static void WinKeyDown()
        {
            keybd_event(LWin, 0, keyDown, 0);
        }

        public static void WinKeyUp()
        {
            keybd_event(LWin, 0, keyUp, 0);
        }
    }
}