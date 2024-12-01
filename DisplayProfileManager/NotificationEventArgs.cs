namespace DisplayProfileManager;
public class NotificationEventArgs(string notificationText) : EventArgs
{
    public string NotificationText { get; } = notificationText;
}