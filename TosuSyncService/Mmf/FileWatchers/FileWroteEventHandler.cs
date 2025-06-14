namespace TosuSyncService.Mmf.FileWatchers;

public delegate void FileWroteEventHandler(object? sender, string file, DateTime modifiedTime);