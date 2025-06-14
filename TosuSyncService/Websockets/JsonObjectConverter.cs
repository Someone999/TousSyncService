using Newtonsoft.Json;

namespace TosuSyncService.Websockets;

public class JsonObjectConverter<T> : IJsonObjectConverter<T>
{
    public T? Create(string source)
    {
        return JsonConvert.DeserializeObject<T>(source);
    }
    
    public bool TryCreate(string source, out T? result)
    {
        try
        {
            result = Create(source);
            return result != null;
        }
        catch (Exception)
        {
            result = default;
            return false;
        }
    }
    
    public static IJsonObjectConverter<T> Instance { get; } = new JsonObjectConverter<T>(); 
}