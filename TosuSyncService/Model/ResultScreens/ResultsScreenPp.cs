using Newtonsoft.Json;

namespace TosuSyncService.Model.ResultScreens;

public class ResultsScreenPp
{
    public double Current { get; set; }
    
    [JsonProperty("fc")]
    public double FullCombo { get; set; }
    
    
}