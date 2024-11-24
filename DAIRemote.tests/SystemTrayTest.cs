namespace DAIRemote.Tests
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;
    using Xunit;
    using Moq;
    using AudioManager;

    public class SystemTrayTest
    {
        [Fact]
        public void HideIconTest()
        {
            using (var form = new Form())
            {
                var manager = new TrayIconManager(form);
                var notifyIconField = typeof(TrayIconManager).GetField("trayIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.NotNull(notifyIconField);

                var notifyIcon = notifyIconField.GetValue(manager) as NotifyIcon;
                Assert.NotNull(notifyIcon);

                notifyIcon.Visible = true;
                manager.HideIcon();
                Assert.False(notifyIcon.Visible);
            }
        }

        [Fact]
        public void ShowIconTest()
        {
            using (var form = new Form())
            {
                var manager = new TrayIconManager(form);
                var notifyIconField = typeof(TrayIconManager).GetField("trayIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.NotNull(notifyIconField);

                var notifyIcon = notifyIconField.GetValue(manager) as NotifyIcon;
                Assert.NotNull(notifyIcon);

                notifyIcon.Visible = false;
                manager.ShowIcon();
                Assert.True(notifyIcon.Visible);
            }
        }


    }
}
