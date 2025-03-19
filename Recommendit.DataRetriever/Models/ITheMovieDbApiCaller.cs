using Recommendit.Result;

namespace DataRetriever.Models;

public interface ITheMovieDbApiCaller
{
    Task<Result<List<ShowData>>> GetApiResponse(string bearerToken, string pageNumber);
    
    Task<string?> PostLogin(string apiKey, string apiUrl);

}