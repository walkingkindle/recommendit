using System.Text.Json.Serialization;

namespace DataRetriever.Models
{
    public class VectorResponse
    {
        [JsonPropertyName("id")]

        public required int Id { get; set; }

        [JsonPropertyName("embedding")]

        public required string Embeddings { get; set; }

    }
}
