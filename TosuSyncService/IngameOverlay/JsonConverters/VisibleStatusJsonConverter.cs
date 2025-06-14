using System.Reflection;
using Newtonsoft.Json;
using TosuSyncService.Mmf;

namespace TosuSyncService.IngameOverlay.JsonConverters;

class VisibleStatusJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        uint flags = 0;
        IEnumerable<string> enums = value as IEnumerable<string> ?? Array.Empty<string>();
        var members = enums as string[] ?? enums.ToArray();
        foreach (var field in typeof(MmfState).GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            if (!members.Contains(field.Name))
            {
                continue;
            }
            
            var val = field.GetValue(null);
            if (val == null)
            {
                continue;
            }

            flags |= (uint)val;
        }

        writer.WriteValue(flags);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.ValueType != typeof(Int64))
        {
            return serializer.Deserialize(reader, objectType);
        }
        
        var readerVal = reader.Value;
        if (readerVal == null)
        {
            throw new Exception();
        }

        MmfState flag = (MmfState)(Int64)readerVal;
        return flag.ToString().Split(',').Select(s => s.Trim()).ToList();
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(IList<string>) == objectType;
    }
}