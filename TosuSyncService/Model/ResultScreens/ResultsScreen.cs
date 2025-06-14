using osuToolsV2.Game.Legacy;
using osuToolsV2.Rulesets.Legacy;
using TosuSyncService.Model.Gameplays;

namespace TosuSyncService.Model.ResultScreens;

public class ResultsScreen
{
    public long ScoreId { get; set; }
    public string PlayerName { get; set; } = "";
    public TosuEnum<LegacyRuleset> Mode { get; set; } = new TosuEnum<LegacyRuleset>();
    public int Score { get; set; } = 0;
    public double? Accuracy { get; set; } = 0;
    public TosuHits Hits { get; set; } = new TosuHits();
    public TosuEnum<LegacyGameMod> Mods { get; set; } = new TosuEnum<LegacyGameMod>();
    public int MaxCombo { get; set; } = 0;
    public string Rank { get; set; } = "";
    public ResultsScreenPp Pp { get; set; } = new ResultsScreenPp();
    public DateTime? CreatedAt { get; set; }
}