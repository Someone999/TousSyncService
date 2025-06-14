using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TosuSyncService.Model.Beatmaps;

public class StarsStatistics
{
    public double RealtimeStars { get; set; }
    public double Total { get; set; }

    public Dictionary<string, double> StarsAttributes { get; set; } = new Dictionary<string, double>();
    internal JObject OriginalObject { get; set; } = new JObject();
    public T? GetAs<T>() where T: StarsStatistics => OriginalObject.ToObject<T>();
}