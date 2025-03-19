using DataRetriever.Models;
using EFCore.BulkExtensions;
using Recommendit.Result;
using Microsoft.Extensions.Logging;
using Recommendit.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DataRetriever.Services;

public class DatabaseOperator:IDatabaseOperator{

    private readonly ShowContext _context;
    private readonly ILogger<DatabaseOperator> _logger;
    public DatabaseOperator(ShowContext context,ILogger<DatabaseOperator> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Result> BulkAddShowsToDatabase(List<Show> shows)
    {
        try
        {
            await _context.BulkInsertAsync(shows);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Result.Failure(DatabaseErrors.DatabaseInsertError);

        }

        return Result.Success();
    }

    public async Task<List<ShowVectorRetrievalDto>> RetrieveVectorsFromAllShows()
    {
        List<ShowVectorRetrievalDto> showVectorRetrievalDto = await _context.Shows
            .Where(c => c.Description.Length != 0)
            .Select(x => new ShowVectorRetrievalDto { Id = x.Id, Description = x.Description })
            .ToListAsync();

        return showVectorRetrievalDto;
    }

    public async Task<Result> SaveVectorsToShowInfo(List<ShowInfo> showInfos)
    {
        try
        {
            await _context.BulkInsertAsync(showInfos);

            await _context.SaveChangesAsync();
        }catch(Exception ex)
        {
            return Result.Failure(DatabaseErrors.DatabaseInsertError);
        }

        return Result.Success();
    }
}
