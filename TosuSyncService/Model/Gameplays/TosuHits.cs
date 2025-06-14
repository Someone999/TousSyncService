using Newtonsoft.Json;

namespace TosuSyncService.Model.Gameplays;

public class TosuHits
{
    [JsonProperty("geki")]
    public int CountGeki { get; set; }
    [JsonProperty("katu")]
    public int CountKatu { get; set; }
    [JsonProperty("300")]
    public int Count300 { get; set; }
    [JsonProperty("100")]
    public int Count100 { get; set; }
    [JsonProperty("50")]
    public int Count50 { get; set; }
    [JsonProperty("0")]
    public int CountMiss { get; set; }
    public int SliderBreaks { get; set; } = 0;
}