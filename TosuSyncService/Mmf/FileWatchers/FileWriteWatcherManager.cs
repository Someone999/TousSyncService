namespace TosuSyncService.Mmf.FileWatchers;

public static class FileWriteWatcherManager
{
    private static Dictionary<string, FileWriteWatcher> _watchers = new Dictionary<string, FileWriteWatcher>();

    public static FileWriteWatcher GetOrCreate(string filePath)
    {
        if (_watchers.TryGetValue(filePath, out var watcher))
        {
            return watcher;
        }

        watcher = new FileWriteWatcher(filePath);
        return watcher;
    }
}