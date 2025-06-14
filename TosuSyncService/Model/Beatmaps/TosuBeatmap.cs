using Newtonsoft.Json;
using osuToolsV2.Database.Beatmap;
using osuToolsV2.Rulesets.Legacy;

namespace TosuSyncService.Model.Beatmaps;

//[JsonConverter(typeof(StarsStatisticJsonConverter))]
public class TosuBeatmap
{
    public TosuBeatmapTime Time { get; set; } = new TosuBeatmapTime();
    public TosuEnum<OsuBeatmapStatus> Status { get; set; } = new TosuEnum<OsuBeatmapStatus>();
    public string Checksum { get; set; } = "";
    [JsonProperty("id")]
    public int BeatmapId { get; set; }
    [JsonProperty("set")]
    public int BeatmapSetId { get; set; }
    public TosuEnum<LegacyRuleset> Mode { get; set; } = new TosuEnum<LegacyRuleset>();
    public string Artist { get; set; } = "";
    public string ArtistUnicode { get; set; } = "";
    public string Title { get; set; } = "";
    public string TitleUnicode { get; set; } = "";
    public string Mapper { get; set; } = "";
    public string Version { get; set; } = "";
    
    [JsonProperty("stats")]
    public TosuBeatmapStatistics Statistics { get; set; } = new TosuBeatmapStatistics();

}