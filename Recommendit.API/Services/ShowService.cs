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


    public async Task<Result<List<Show>>> GetRecommendedShowsWithCosineSimilarity(List<int> showIds, int topN = 5)
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

        return Result<List<Show>>.Success(recommendedShows.Value);


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

    private async Task<Result<List<ShowInfo>>> GetShowInfos()
    {
        List<ShowInfo> showInfos = await _context.ShowInfos
            .AsNoTracking()
            .Select(x => new ShowInfo
            {
                ShowId = x.ShowId, 
                VectorDouble = x.VectorDouble
            })
            .ToListAsync();

        if (showInfos.IsNullOrEmpty())
        {
            return Result<List<ShowInfo>>.Failure(DatabaseErrors.NullCollectionError);
        }

        return Result<List<ShowInfo>>.Success(showInfos);
    }



    public async Task<Result<List<Show>>> GetShowsExactMatchingRecordsAsync(string input)
    {
        var exactmatchedRecords = await _context.Shows.AsNoTracking().Where(s => s.Name == input)
               .Select(s => new Show
               {
                   Id = s.Id,
                   Name = s.Name,
                   Description = s.Description,
                   ImageUrl = s.ImageUrl,
                   ReleaseYear = s.ReleaseYear,
                   Score = s.Score,
                   OriginalCountry = s.OriginalCountry,
                   OriginalLanguage = s.OriginalLanguage
               }).ToListAsync();

        return exactmatchedRecords.IsNullOrEmpty()
            ? Result<List<Show>>.Failure(DatabaseErrors.NullCollectionError)
            : Result<List<Show>>.Success(exactmatchedRecords);


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