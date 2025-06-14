using Newtonsoft.Json;

namespace TosuSyncService.Model.Gameplays;

public class GamePlayPp
{
    public double Current { get; set; }
    
    [JsonProperty("fc")]
    public double FullComboPp { get; set; }
    
    [JsonProperty("maxAchievedThisPlay")]
    public double PlayerMaxPp { get; set; }

    public GamePlayPpDetail Detailed { get; set; } = new GamePlayPpDetail();
}