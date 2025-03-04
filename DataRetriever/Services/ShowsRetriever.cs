using System.Text;
using System.Text.Json;
using AutoMapper;
using DataRetriever.Models;
using Microsoft.Extensions.Options;
using Recommendit.Result;
using ShowPulse.Models;

namespace DataRetriever.Services;

public class ShowsRetriever : IShowsRetriever
{


    private readonly IMapper _mapper;
    private readonly ILogger<ShowsRetriever> _logger;
    private readonly IDatabaseInserter _databaseInserter;
    private readonly ITheMovieDbApiCaller _theMovieDbApiCaller;
    private readonly IConfiguration _configuration;
    private readonly IOptions<TvDbSettings> _tvDbSettings;

    public ShowsRetriever(IMapper mapper, ILogger<ShowsRetriever> logger, ITheMovieDbApiCaller theMovieDbApiCaller,
        IDatabaseInserter databaseInserter, IConfiguration configuration, IOptions<TvDbSettings> tvDbSettings)
    {
        _mapper = mapper;
        _logger = logger;
        _theMovieDbApiCaller = theMovieDbApiCaller;
        _databaseInserter = databaseInserter;
        _configuration = configuration;
        _tvDbSettings = tvDbSettings;
    }

    public async Task<Result> AddShows()
    {
        var bearerToken = await PostLogin();

        if (bearerToken == null)
        {
            return Result.Failure(APIErrrors.LoginError);
        }
        var apiResponse = _theMovieDbApiCaller.GetApiResponse(bearerToken,"1");
        if (apiResponse.Result.IsFailure)
        {
            return Result.Failure(apiResponse.Result.Error);
        }

        var showsResult = MapResponseJsonToShow(apiResponse.Result.Value);

        if (showsResult.IsFailure)
        {
            return Result.Failure(showsResult.Error);
        }

        var databaseAddShowsResult = _databaseInserter.BulkAddShowsToDatabase(showsResult.Value);

        if (databaseAddShowsResult.Result.IsFailure)
        {
            return Result.Failure(databaseAddShowsResult.Result.Error);
        }


        return Result.Success();


    }

    private Result<List<Show>> MapResponseJsonToShow(List<ShowDataDto> showsData)
    {
        List<Show> shows = new List<Show>();
        foreach (var showData in showsData)
        {
            try
            {
                var show = _mapper.Map<Show>(showData);
                shows.Add(show);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return Result<List<Show>>.Failure(APIErrrors.ApiResponseError);
            }
        }

        return Result<List<Show>>.Success(shows);
    }

    public async Task<string?> PostLogin()
    {

        string token = await _theMovieDbApiCaller.PostLogin(_tvDbSettings.Value.ApiKey, _tvDbSettings.Value.ApiUrl);

        return token;

    }
}
    
   