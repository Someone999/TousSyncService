using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;

namespace TosuSyncService.BeatmapCaches;

public class DifficultyAttributeItem(Ruleset ruleset)
{
    public Ruleset Ruleset { get; set; } = ruleset;

    public Dictionary<Mod[], DifficultyAttributes> DifficultyAttributesMap { get; } =
        new Dictionary<Mod[], DifficultyAttributes>(comparer: new NameModSequenceEqualityComparer());
}