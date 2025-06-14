using osu.Game.Rulesets.Mods;

namespace TosuSyncService.BeatmapCaches;

public class NameModSequenceEqualityComparer : IEqualityComparer<Mod[]>
{
    public bool Equals(Mod[]? x, Mod[]? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        if (x.Length != y.Length)
        {
            return false;
        }

        HashSet<string> slots = new HashSet<string>();
        foreach (var xMod in x)
        {
            slots.Add(xMod.Name);
        }
        
        return y.All(m => slots.Contains(m.Name));
    }

    public int GetHashCode(Mod[] obj)
    {
        HashCode hashCode = new HashCode();
        foreach (var mod in obj)
        {
            hashCode.Add(mod.Name);
        }

        return hashCode.ToHashCode();
    }
}