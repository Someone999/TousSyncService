using Newtonsoft.Json;

namespace TosuSyncService.Model.Settings;

public class KeyBindSettings
{
    public OsuKeyBindSettings Osu { get; set; } = new OsuKeyBindSettings();
    public TaikoKeyBindSettings Taiko { get; set; } = new TaikoKeyBindSettings();
    
    [JsonProperty("fruits")]
    public CatchKeyBindSettings Catch { get; set; } = new CatchKeyBindSettings();
    public string QuickRetry { get; set; } = "";
}