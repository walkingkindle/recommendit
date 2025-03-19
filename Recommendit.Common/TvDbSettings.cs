namespace DataRetriever.Models
{
    public class TvDbSettings
    {
        public required string ApiKey { get; init; }

        public required string ApiUrl { get; init; }

        public string? AccessToken { get; init; }
    }
}
