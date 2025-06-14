using Newtonsoft.Json;

namespace TosuSyncService.Model.Performances;

public class AccuracyPerformance
{
    [JsonProperty("100")]
    public double Full { get; set; }
    
    [JsonProperty("99")]
    public double Percent99 { get; set; }
    
    [JsonProperty("98")]
    public double Percent98 { get; set; }
    
    [JsonProperty("97")]
    public double Percent97 { get; set; }
    
    [JsonProperty("96")]
    public double Percent96 { get; set; }
    
    [JsonProperty("95")]
    public double Percent95 { get; set; }
}