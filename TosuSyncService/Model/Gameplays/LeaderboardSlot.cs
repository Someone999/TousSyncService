using Newtonsoft.Json;
using osuToolsV2.Online.OsuApi.Version1.Multiplayer.Matches;

namespace TosuSyncService.Model.Gameplays;

public class LeaderboardSlot
{
    public bool IsFailed { get; set; }
    public int Position { get; set; }
    public MatchTeam Team { get; set; }
    
    [JsonProperty("name")]
    public string PlayerName { get; set; } = "";
    public int Score { get; set; } = 0;
    public double Accuracy { get; set; } = 0;
    public TosuHits Hits { get; set; } = new TosuHits();
    
    public static LeaderboardSlot Empty { get; } = new LeaderboardSlot();
    public static LeaderboardSlot CreateLastPlayerSlot()  => new LeaderboardSlot()
    {
        Position = 52,
        PlayerName = "-"
    };
}