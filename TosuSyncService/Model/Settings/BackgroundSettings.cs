using Newtonsoft.Json;

namespace TosuSyncService.Model.Settings;

public class BackgroundSettings
{
    public int Dim { get; set; }
    
    [JsonProperty("video")]
    public bool EnableVideo { get; set; }
    
    [JsonProperty("storyboard")]
    public bool EnableStoryboard { get; set; }
}