namespace DAIRemote.Tests
{
    using Moq;
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Xunit;

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

        [Fact]
        public async Task Constructor_Timeout()
        {
            var mockForm = new Mock<Form>();
            var timeout = TimeSpan.FromMilliseconds(500);
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            var task = Task.Run(() =>
            {
                var trayIconManager = new TrayIconManager(mockForm.Object);
            }, token);

            var completedWithinTimeout = await Task.WhenAny(task, Task.Delay(timeout)) == task;
            if (!completedWithinTimeout)
            {
                cancellationTokenSource.Cancel();
            }

            Assert.True(completedWithinTimeout, "TrayIconManager constructor did not complete within the timeout.");
        }

        [Fact]
        public async Task TrayMenu_Timeout()
        {
            var mockForm = new Mock<Form>();
            var trayIconManager = new TrayIconManager(mockForm.Object);
            var trayMenu = new ContextMenuStrip();

            var timeout = TimeSpan.FromSeconds(5);
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            var populateTrayMenuMethod = typeof(TrayIconManager).GetMethod("PopulateTrayMenu", BindingFlags.NonPublic | BindingFlags.Instance);

            var task = Task.Run(() =>
            {
                populateTrayMenuMethod.Invoke(trayIconManager, new object[] { trayMenu });
            }, token);

            var completedWithinTimeout = await Task.WhenAny(task, Task.Delay(timeout)) == task;
            if (!completedWithinTimeout)
            {
                cancellationTokenSource.Cancel();
            }

            Assert.True(completedWithinTimeout, "Tray menu was not populated within the 5-second timeout.");
            Assert.NotEmpty(trayMenu.Items);
        }


        [Fact]
        public void TrayIconManager_HasShowIcon()
        {
            var mockForm = new Mock<Form>();
            var trayIconManager = new TrayIconManager(mockForm.Object);

            var method = trayIconManager.GetType().GetMethod("ShowIcon");
            Assert.NotNull(method);
        }

        [Fact]
        public async Task OnShow_Timeout()
        {
            var form = new System.Windows.Forms.Form();
            var trayIconManager = new TrayIconManager(form);

            TimeSpan timeout = TimeSpan.FromSeconds(2);

            var task = Task.Run(() => trayIconManager.OnShow(this, EventArgs.Empty));

            var completedInTime = await Task.WhenAny(task, Task.Delay(timeout)) == task;

            Assert.True(completedInTime, "The OnShow method did not complete within the expected timeout.");

            Assert.Equal(System.Windows.Forms.FormWindowState.Normal, form.WindowState);
        }



    }
}
