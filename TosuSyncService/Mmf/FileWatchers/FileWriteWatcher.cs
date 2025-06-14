namespace TosuSyncService.Mmf.FileWatchers;

public class FileWriteWatcher(string file)
{

    private DateTime _lastModifiedTime;

    public void Check()
    {
        var currentModifiedTime = File.GetLastWriteTime(file);
        if (currentModifiedTime == _lastModifiedTime)
        {
            return;
        }
        
        FileWrote?.Invoke(this, file, currentModifiedTime);
        _lastModifiedTime = currentModifiedTime;
    }

    public event FileWroteEventHandler? FileWrote;
}