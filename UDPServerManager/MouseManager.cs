using System.Runtime.InteropServices;

namespace UDPServerManager
{
    class MouseManager
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010,
            Wheel = 0x00000800
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            GetCursorPos(out MousePoint cursorPos);
            return cursorPos;
        }

        public static void MouseEvent(MouseEventFlags value, int data = 0)
        {
            MousePoint position = GetCursorPosition();
            mouse_event((int)value, position.X, position.Y, data, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint(int x, int y)
        {
            public int X = x;
            public int Y = y;
        }
    }
}
