using Newtonsoft.Json;

namespace TosuSyncService.Model.Beatmaps;

public class TosuBeatmapTime
{
    [JsonProperty("live")]
    public double CurrentTime { get; set; }
    public int FirstObject { get; set; }
    public int LastObject { get; set; }
}