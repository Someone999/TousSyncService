using Newtonsoft.Json;

namespace TosuSyncService.Model.Gameplays;

public class GamePlayRank
{
    public string Current { get; set; } = "";
    
    [JsonProperty("maxThisPlay")]
    public string PlayerMaxRank { get; set; } = "";
}