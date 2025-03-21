using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Recommendit.DataRetriever.Models;
using Recommendit.Infrastructure;

namespace Recommendit.DataRetriever.Services
{
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoCollection<ShowInfoEssentials> _showCollection;
        private readonly ShowContext _context;

        public MongoDbService(IMongoDatabase database, ShowContext context)
        {
            _showCollection = database.GetCollection<ShowInfoEssentials>("ShowInfos");
            _context = context;
        }

        public async Task<List<ShowInfoEssentials>> GetShowInfoVectorsAsync()
        {
           return await _showCollection.Find(p => true).ToListAsync();
        }

        public async Task SetShowInfoVectorsAsync()
        {
            var showInfos = await _context.ShowInfos.Select(x => new ShowInfoEssentials { ShowId = x.ShowId, VectorDouble = Recommendit.Helpers.CollectionExtensions.ConvertStringVectorToDoubleArray(x.VectorDouble) }).ToListAsync();

            await _showCollection.InsertManyAsync(showInfos);


        }
    }
}
