using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Recommendit.Models;
using Recommendit.Result;
using Recommendit.Interface;
using Recommendit.Helpers;
using Recommendit.Infrastructure;
using Recommendit.DataRetriever.Models;

namespace ShowPulse.Services;

public class ShowService : IShowService
{

    private readonly ShowContext _context;

    private readonly IVectorService _vectorService;

    private readonly IMongoDbService _dbService;

    public ShowService(ShowContext dbContext, IVectorService vectorService, IMongoDbService dbService)
    {
        _context = dbContext;
        _vectorService = vectorService;
        _dbService = dbService;
    }


    public async Task<Result<List<Show>>> GetRecommendedShowsWithCosineSimilarity(List<int> showIds, int topN = 10)
    {

        var allShows = await _dbService.GetShowInfoVectorsAsync();

        var userShowsVectors = allShows
            .Where(showInfo => showIds.Contains(showInfo.ShowId))
            .Select(p=> p.VectorDouble)
            .ToList();
        double[] averageVector = _vectorService.CalculateAverageVector(userShowsVectors);
        List<int> recommendedShowIds = await _vectorService.GetSimilarities(allShows, averageVector, topN);
        List<int> filteredRecommendedShowIds = recommendedShowIds.Except(showIds).ToList();

        var recommendedShows = await GetRecommendedShowByIds(filteredRecommendedShowIds);

        return recommendedShows.IsSuccess ? Result<List<Show>>.Success(recommendedShows.Value) : Result<List<Show>>.Failure(APIErrrors.ApiResponseError);
    }



    private async Task<Result<List<Show>>> GetRecommendedShowByIds(List<int> showIds)
    {
        var shows = await _context.Shows.AsNoTracking().Where(x=> showIds.Contains(x.Id)).ToListAsync();

        if (shows.IsNullOrEmpty())
        {
            return Result<List<Show>>.Failure(DatabaseErrors.NullCollectionError);
        }

        return Result<List<Show>>.Success(shows);

    }

    public async Task<Result<Show>> GetShowByIdAsync(int showId)
    {
        var show = await _context.Shows.FirstOrDefaultAsync(x => x.Id == showId);

        if (show == null)
        {
            return Result<Show>.Failure(DatabaseErrors.NullShowError);
        }

        return Result<Show>.Success(show);

    }


    public async Task<Result<List<Show>>> GetMatchingShowRecordsAsync(string input)
    {
         var matchedRecords = await _context.Shows.Where(s => s.Name.StartsWith(input))
                    .Take(10)
                    .Select(s => new Show
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        ReleaseYear = s.ReleaseYear,
                        ImageUrl = s.ImageUrl
                    }).ToListAsync();

         if (matchedRecords.IsNullOrEmpty())
         {
             return Result<List<Show>>.Failure(DatabaseErrors.NullCollectionError);
         }

        return Result<List<Show>>.Success(matchedRecords);

    }


}