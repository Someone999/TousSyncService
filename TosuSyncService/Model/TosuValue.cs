using Newtonsoft.Json;

namespace TosuSyncService.Model;

public class TosuValue<T> where T : notnull
{
    [JsonProperty("number")]
    public T? Value { get; set; }

    [JsonProperty("name")]
    public string StringValue { get; set; } = "";
}