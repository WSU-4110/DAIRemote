using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using DisplayProfileManager;

namespace DAIRemote
{
	public class MonitorControl
	{
		public const int WM_SYSCOMMAND = 0x0112;
		public const int SC_MONITORPOWER = 0xF170;
		public const int MONITOR_OFF = 2;
		public const int MONITOR_ON = -1;
		public const int HWND_BROADCAST = 0xFFFF;

		[DllImport("user32.dll")]
		private static extern int PostMessage(int hWnd, int hMsg, int wParam, int lParam);

		private NotifyIcon trayIcon;

		public MonitorControl(NotifyIcon trayIcon)
		{
			this.trayIcon = trayIcon;
		}

		public void TurnOffMonitors()
		{
			PostMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_OFF);
		}

		public void TurnOnMonitors()
		{
			Task.Run(() =>
			{
				PostMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_ON);
				System.Threading.Thread.Sleep(100);
				RefreshTrayIcon();
			});
		}

		private void RefreshTrayIcon()
		{
			trayIcon.Visible = false;
            trayIcon.Visible = true;

		}


        public static void LoadDisplayProfile(string profilePath)
        {
            DisplayProfileManager.DisplayConfig.DISPLAYCONFIG_PATH_INFO[] pathInfoArray;
            DisplayProfileManager.DisplayConfig.DISPLAYCONFIG_MODE_INFO[] modeInfoArray;
            DisplayProfileManager.DisplayConfig.MonitorAdditionalInfo[] additionalInfo;

            bool status = DisplayProfileManager.DisplayConfig.LoadDisplaySettings(profilePath, out pathInfoArray, out modeInfoArray, out additionalInfo);

            if (status)
            {
                bool applySuccess = DisplayProfileManager.DisplayConfig.SetDisplaySettings(profilePath);

                if (!applySuccess)
                {
                    MessageBox.Show("Failed to apply the display settings.");
                }
            }
            else
            {
                MessageBox.Show("Failed to load the display profile.");
            }
        }


    }
}
