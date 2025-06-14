using Newtonsoft.Json;
using osuToolsV2.Game.Legacy;
using osuToolsV2.Rulesets.Legacy;

namespace TosuSyncService.Model.Gameplays;

public class GamePlay
{
    public string PlayerName { get; set; } = "";
    public TosuEnum<LegacyRuleset> Mode { get; set; } = new TosuEnum<LegacyRuleset>();
    public int Score { get; set; } = 0;
    public double Accuracy { get; set; } = 0;
    public HealthBar HealthBar { get; set; } = new HealthBar();
    public TosuHits Hits { get; set; } = new TosuHits();
    
    [JsonProperty("hitErrorArray")]
    public List<double> HitErrors { get; set; } = new List<double>();
    public GamePlayCombo Combo { get; set; } = new GamePlayCombo();
    public TosuEnum<LegacyGameMod> Mods { get; set; } = new TosuEnum<LegacyGameMod>();
    public GamePlayRank Rank { get; set; } = new GamePlayRank();
    public GamePlayPp Pp { get; set; } = new GamePlayPp();
    public double UnstableRate { get; set; } = 0;
}