using Newtonsoft.Json;

namespace TosuSyncService.Model.Gameplays;

public class GamePlayCombo
{
    public int Current { get; set; }
    [JsonProperty("max")]
    public int MaxCombo { get; set; }
}