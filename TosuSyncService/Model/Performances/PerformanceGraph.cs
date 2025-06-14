using Newtonsoft.Json;

namespace TosuSyncService.Model.Performances;

public class PerformanceGraph
{
    public List<PerformanceSeries> Series { get; set; } = new List<PerformanceSeries>();
    [JsonProperty("xaxis")]
    public List<double> XAxis { get; set; } = new List<double>();
}