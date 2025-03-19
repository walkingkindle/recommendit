using Recommendit.Infrastructure;
using Recommendit.Result;

namespace DataRetriever.Models;

public interface IShowsRetriever
{
    
    Task<Result> AddShows();
    
    Task<string?> PostLogin();

    Task<Result> InsertShowsToDatabase(List<Show> shows);


    Task<Result> GetShowVectorValue();

}