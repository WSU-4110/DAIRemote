public static class DisplayProfileWatcher
{
    private static FileSystemWatcher fileSystemWatcher;
    private static readonly object LockObject = new();

    public static event FileSystemEventHandler ProfileChanged;

    public static void Initialize(string directoryPath)
    {
        if (fileSystemWatcher != null) return;

        lock (LockObject)
        {
            if (fileSystemWatcher == null)
            {
                fileSystemWatcher = new FileSystemWatcher(directoryPath)
                {
                    NotifyFilter = NotifyFilters.FileName,
                    EnableRaisingEvents = true
                };

                fileSystemWatcher.Created += OnProfileChanged;
                fileSystemWatcher.Deleted += OnProfileChanged;
                fileSystemWatcher.Renamed += OnProfileChanged;
            }
        }
    }

    private static void OnProfileChanged(object sender, FileSystemEventArgs e)
    {
        ProfileChanged?.Invoke(sender, e);
    }
}