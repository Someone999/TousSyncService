using Newtonsoft.Json;

namespace TosuSyncService.Model.Settings;

public struct OffsetSettings
{
    [JsonProperty("universal")]
    public int Universal { get; set; }
}