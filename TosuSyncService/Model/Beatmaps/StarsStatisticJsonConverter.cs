using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osuToolsV2.Rulesets.Legacy;
using TosuSyncService.Model.Gameplays;

namespace TosuSyncService.Model.Beatmaps;

class StarsStatisticJsonConverter : JsonConverter<StarsStatistics>
{
    public override void WriteJson(JsonWriter writer, StarsStatistics? value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        if (value == null)
        {
            writer.WriteNull();
            writer.WriteEndObject();
            return;
        }
        foreach (var attribute in value.StarsAttributes)
        {
            writer.WritePropertyName(attribute.Key);
            writer.WriteValue(attribute.Value);
        }
        
        writer.WriteEndObject();
    }

    public override StarsStatistics ReadJson(JsonReader reader, Type objectType, StarsStatistics? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        var realtimeStars = jObject["live"]?.ToObject<double>();
        var totalStars = jObject["total"]?.ToObject<double>();
        StarsStatistics starsStatistics = existingValue ?? new StarsStatistics();
        starsStatistics.RealtimeStars = realtimeStars ?? 0;
        starsStatistics.Total = totalStars ?? 0;
        foreach (var (k, v) in jObject)
        {
            starsStatistics.StarsAttributes.Add(k, v?.ToObject<double>() ?? 0);
        }
        
        starsStatistics.OriginalObject = jObject;
        return starsStatistics;
    }
}