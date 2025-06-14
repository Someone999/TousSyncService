using HsManCommonLibrary.NestedValues;
using HsManCommonLibrary.NestedValues.NestedValueAdapters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TosuSyncService.Mmf.Builders;

public class JsonMmfConfigReader : IMmfItemConfigReader
{
    private static readonly JsonNestedValueStoreAdapter Adapter = new JsonNestedValueStoreAdapter();

    public void LoadConfig(string file)
    {
        if (Config != null)
        {
            return;
        }
        
        var content = File.ReadAllText(file);
        var jObject = JsonConvert.DeserializeObject<JObject>(content);

        Config = Adapter.ToNestedValue(jObject ?? throw new InvalidOperationException("Invalid config file."));
    }

    public INestedValueStore? Config { get; private set; }
}