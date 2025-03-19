using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Recommendit.Models;
using Recommendit.Result;
using Recommendit.Interface;
using Recommendit.Helpers;
using Recommendit.Infrastructure;

namespace ShowPulse.Services;

public class ShowService : IShowService
{

    private readonly ShowContext _context;

    private readonly IVectorService _vectorService;

    public ShowService(ShowContext dbContext)
    {
        _context = dbContext;
    }

    
    //public async Task<Result<List<Show>>> GetRecommendedShowsWithCosineSimilarity(List<int> showIds,int topN=5)
    //{
    //    var allShows = GetShowInfos();

    //    if (allShows.Result.IsFailure)
    //    {
    //        return Result<List<Show>>.Failure(allShows.Result.Error);
    //    }
    //    var userShowsVectors = allShows.Result.Value.Where(show => showIds.Contains(show.Id));
    //    double[] averageVector = _vectorService.CalculateAverageVector(userShowsVectors.Select(x=> x.VectorDouble).ToList());
    //    List<int> recommendedShowIds = await _vectorService.GetSimilarities(allShows.Result.Value, averageVector, topN);
    //    List<int> filteredRecommendedShowIds = recommendedShowIds.Except(showIds).ToList();

    //    var recommendedShows = await GetRecommendedShowByIds(showIds);
        
    //    return Result<List<Show>>.Success(recommendedShows.Value);
       
       
    //}

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
        // Use Include to load the related ShowInfo entities
        List<ShowInfo> showInfos = await _context.Shows
            .AsNoTracking()
            .Include(x => x.ShowInfo) // Include the ShowInfo
            .Select(x => new ShowInfo
            {
                Id = x.ShowInfo.Id, // Use the ShowInfo Id
                VectorDouble = x.ShowInfo.VectorDouble // Access the VectorDouble from ShowInfo
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
                   FinalEpisodeAired = s.FinalEpisodeAired,
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
         var matchedRecords = await _context.Shows.Where(s => s.Name.Contains(input))
                    .Take(10).Select(s => new Show
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