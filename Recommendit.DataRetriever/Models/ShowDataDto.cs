using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DataRetriever.Models;
    public class ShowData
    {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("overview")] public string Overview { get; set; }

        [JsonPropertyName("image")] public string Image { get; set; }

        [JsonPropertyName("firstAired")] public string FirstAired { get; set; }

        [JsonPropertyName("lastAired")] public string LastAired { get; set; }

        [JsonPropertyName("score")] public int? Score { get; set; }

        [JsonPropertyName("status")] public StatusInfo Status { get; set; }

        [JsonPropertyName("originalCountry")] public string OriginalCountry { get; set; }

        [JsonPropertyName("originalLanguage")] public string OriginalLanguage { get; set; }

        [JsonPropertyName("year")] public string Year { get; set; }
    }

    public class StatusInfo
    {
        [JsonPropertyName("id")] public int? Id { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }
    }