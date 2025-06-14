using HsManCommonLibrary.Caches;
using osu.Game.Beatmaps;

namespace TosuSyncService.BeatmapCaches;

public class DifficultyAttributeCache : MemoryCache<IWorkingBeatmap, DifficultyAttributeItem>
{
}