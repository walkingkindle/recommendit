using Recommendit.Infrastructure;
using Recommendit.Result;

namespace Recommendit.Interface
{
    public interface IShowService
    {
        //Task<Result<List<Show>>> GetRecommendedShowsWithCosineSimilarity(List<int> showIds, int topN = 5);


        Task<Result<List<Show>>> GetShowsExactMatchingRecordsAsync(string input);


        Task<Result<Show>> GetShowByIdAsync(int showId);


        Task<Result<List<Show>>> GetMatchingShowRecordsAsync(string input);





    }
}
