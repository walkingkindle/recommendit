using Newtonsoft.Json;

namespace DataRetriever.Models;

public class ShowStatus
{
    [JsonProperty("id")]
    public int? Id { get; set; }
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("recordType")]
    public string? RecordType { get; set; }

    [JsonProperty("keepUpdated")]
    public bool KeepUpdated { get; set; }

}