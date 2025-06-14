using HsManCommonLibrary.Caches;
using TosuSyncService.Model;
using TosuSyncService.Model.Beatmaps;

namespace TosuSyncService.Utils;

public static class BeatmapUtils
{
    private static MemoryCache<string, string> _pathCache = new MemoryCache<string, string>();

    private static bool TryGetFromCache(string md5, out string? path)
    {
        path = _pathCache.GetValue(md5);
        return path != null;
    }

    public static string? GetCurrentBeatmapPath(TosuData gosuData, bool ignoreCache = false)
    {
        if (!ignoreCache && TryGetFromCache(gosuData.Beatmap.Checksum, out var path))
        {
            return path;
        }
        
        string songFolder = gosuData.Folders.Songs;
        TosuBeatmap beatmap = gosuData.Beatmap;
        var beatmapFileName = gosuData.DirectPath.BeatmapFile;
        var fullPath = Path.Combine(songFolder, beatmapFileName);
        _pathCache.Add(beatmap.Checksum, fullPath);
        return File.Exists(fullPath) ? fullPath : null;
    }
    
    /*public static string? GetBeatmapPath(TosuData gosuData, TosuBeatmap beatmap, bool ignoreCache = false)
    {
        if (!ignoreCache && TryGetFromCache(gosuData.Beatmap.Checksum, out var path))
        {
            return path;
        }

        string songFolder = gosuData.Folders.Songs;
        var beatmapFolder = beatmap.Path.FolderName;
        var beatmapFileName = beatmap.Path.FileName;
        var fullPath = Path.Combine(songFolder, beatmapFolder, beatmapFileName);
        _pathCache.Add(beatmap.Md5, fullPath);
        return File.Exists(fullPath) ? fullPath : null;
    }*/
}