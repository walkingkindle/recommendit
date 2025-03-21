using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Recommendit.Infrastructure
{
    public class ShowInfoEssentials
    {
        [JsonPropertyName("_id")]
        [BsonId]
        public ObjectId Id { get; set; }

        [JsonPropertyName("ShowId")]
        public int ShowId { get; set; }

        [JsonPropertyName("VectorDouble")]
        public required double[] VectorDouble { get; set; }
    }
}
