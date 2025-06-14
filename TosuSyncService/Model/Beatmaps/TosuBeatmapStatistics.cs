using Newtonsoft.Json;
using TosuSyncService.Model.Gameplays;

namespace TosuSyncService.Model.Beatmaps;

public class TosuBeatmapStatistics
{
    public StarsStatistics Stars { get; set; } = new StarsStatistics();
    
    [JsonProperty("ar")]
    public ConvertibleValue<double> ApproachRate { get; set; } = new ConvertibleValue<double>();
    
    [JsonProperty("cs")]
    public ConvertibleValue<double> CircleSize { get; set; } = new ConvertibleValue<double>();
    
    [JsonProperty("hp")]
    public ConvertibleValue<double> HpDrain { get; set; } = new ConvertibleValue<double>();
    
    [JsonProperty("od")]
    public ConvertibleValue<double> OverallDifficulty { get; set; } = new ConvertibleValue<double>();
    public BeatmapObjectsStatistics Objects { get; set; } = new BeatmapObjectsStatistics();
    public int MaxCombo { get; set; }
}