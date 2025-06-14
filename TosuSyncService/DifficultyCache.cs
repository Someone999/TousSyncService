using HsManCommonLibrary.Locks;
using osu.Game.Rulesets.Difficulty;

namespace TosuSyncService;
using Mod = osu.Game.Rulesets.Mods.Mod;
public class DifficultyCache
{
    public DifficultyCalculator? DifficultyCalculator { get; set; }
    private  osu.Game.Rulesets.Difficulty.DifficultyAttributes? _cached;
    private IEnumerable<Mod> _cachedMods = Array.Empty<Mod>();
    private LockManager _lockManager = new LockManager();
    public bool Updating { get; private set; }
    
    public bool HasSameMod(IEnumerable<Mod> mods)
    {
        var enumerable = mods as Mod[] ?? mods.ToArray();
        return _cachedMods.SequenceEqual(enumerable);
    }
    public void Update(IEnumerable<Mod> mods)
    {
        var enumerable = mods as Mod[] ?? mods.ToArray();
        
        lock (_lockManager.AcquireLockObject("UpdateLock"))
        {
            Updating = true;
            _cached = DifficultyCalculator?.Calculate(enumerable);
            _cachedMods = enumerable;
            Updating = false;
        }
    }

    public osu.Game.Rulesets.Difficulty.DifficultyAttributes? GetCached()
    {
        return Updating ? null : _cached;
    } 
}