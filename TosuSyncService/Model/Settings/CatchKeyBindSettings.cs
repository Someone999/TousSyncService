using Newtonsoft.Json;

namespace TosuSyncService.Model.Settings;

public class CatchKeyBindSettings
{
    [JsonProperty("k1")]
    public string Key1 { get; set; } = "";
    
    [JsonProperty("k2")]
    public string Key2 { get; set; } = "";
    
    public string Dash { get; set; } = "";
}