using Newtonsoft.Json;

namespace TosuSyncService.Model.Gameplays;

public class GamePlayPpDetail
{
    public GamePlayPpAttribute Current { get; set; } = new GamePlayPpAttribute();
    
    [JsonProperty("fc")]
    public GamePlayPpAttribute FullCombo { get; set; } = new GamePlayPpAttribute();
}