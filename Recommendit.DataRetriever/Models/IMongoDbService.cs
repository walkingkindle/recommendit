using Recommendit.Infrastructure;

namespace Recommendit.DataRetriever.Models
{
    public interface IMongoDbService
    {
        public Task SetShowInfoVectorsAsync();

        public Task<List<ShowInfoEssentials>> GetShowInfoVectorsAsync();
    }
}
