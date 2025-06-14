using Newtonsoft.Json;

namespace TosuSyncService.Model;

public class ManiaKeyOverlay
{
    public int Size { get; set; }

    [JsonProperty("array")]
    public List<ManiaKeyState> KeyStates { get; set; } = new List<ManiaKeyState>();
}