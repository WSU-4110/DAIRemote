namespace DisplayProfileManager;
public class NotificationEventArgs : EventArgs
{
    public string NotificationText { get; }

    public NotificationEventArgs(string notificationText)
    {
        NotificationText = notificationText;
    }
}