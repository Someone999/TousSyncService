using Newtonsoft.Json;

namespace TosuSyncService.Model.Settings;

public struct Resolution
{
    public int Width { get; set; }
    public int Height { get; set; }
    public bool FullScreen { get; set; }
    
    [JsonProperty("fullscreenWidth")]
    public int FullScreenWidth { get; set; }
    
    [JsonProperty("fullscreenHeight")]
    public int FullScreenHeight { get; set; }
}