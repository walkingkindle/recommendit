using System.Text;
using System.Text.Json;
using AutoMapper;
using DataRetriever.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recommendit.Infrastructure;
using Recommendit.Result;
using System.Linq;
using Recommendit.Helpers;

namespace DataRetriever.Services;

public class ShowsRetriever : IShowsRetriever
{


    private readonly IMapper _mapper;
    private readonly ILogger<ShowsRetriever> _logger;
    private readonly IDatabaseOperator _databaseInserter;
    private readonly ITheMovieDbApiCaller _theMovieDbApiCaller;
    private readonly IConfiguration _configuration;
    private readonly IOptions<TvDbSettings> _tvDbSettings;
    private readonly HttpClient _httpClient;

    public ShowsRetriever(IMapper mapper, ILogger<ShowsRetriever> logger, ITheMovieDbApiCaller theMovieDbApiCaller,
        IDatabaseOperator databaseInserter, IConfiguration configuration, IOptions<TvDbSettings> tvDbSettings, HttpClient client)
    {
        _mapper = mapper;
        _logger = logger;
        _theMovieDbApiCaller = theMovieDbApiCaller;
        _databaseInserter = databaseInserter;
        _configuration = configuration;
        _tvDbSettings = tvDbSettings;
        _httpClient = client;
        
    }

    public async Task<Result> AddShows()
    {
        var bearerToken = await PostLogin();

        List<Show> allShows = new List<Show>();

        if (bearerToken == null)
        {
            return Result.Failure(APIErrrors.LoginError);
        }

        for (int i = 1; i < 2; i++)
        {
            var apiResponse = _theMovieDbApiCaller.GetApiResponse(bearerToken, "76");
            if (apiResponse.Result.IsFailure)
            {
                return Result.Failure(apiResponse.Result.Error);
            }

            var showsResult = MapResponseJsonToShow(apiResponse.Result.Value);

            if (showsResult.IsFailure)
            {
                return Result.Failure(showsResult.Error);
            }

            allShows.AddRange(showsResult.Value);
        }

        await InsertShowsToDatabase(allShows);


        return Result.Success();


    }

    private Result<List<Show>> MapResponseJsonToShow(List<ShowData> showsData)
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
                continue;
                //return Result<List<Show>>.Failure(APIErrrors.ApiResponseError);
            }
        }

        return Result<List<Show>>.Success(shows);
    }

    public async Task<string?> PostLogin()
    {

        if(string.IsNullOrEmpty(_tvDbSettings.Value.ApiKey) || string.IsNullOrEmpty(_tvDbSettings.Value.ApiUrl))
        {
            throw new Exception("Missing configuration files");
        }

        if (!string.IsNullOrEmpty(_tvDbSettings.Value.AccessToken))
        {
            return _tvDbSettings.Value.AccessToken;
        }

        string token = await _theMovieDbApiCaller.PostLogin(_tvDbSettings.Value.ApiKey, _tvDbSettings.Value.ApiUrl);

        return token;

    }

    public async Task<Result> InsertShowsToDatabase(List<Show> shows)
    {
         var databaseAddShowsResult = await _databaseInserter.BulkAddShowsToDatabase(shows);

        if (databaseAddShowsResult.IsFailure)
        {
            return Result.Failure(databaseAddShowsResult.Error);
        }

        return Result.Success();
    }

    public async Task<Result> GetShowVectorValue()
    {
        List<ShowVectorRetrievalDto> dto = await _databaseInserter.RetrieveVectorsFromAllShows();

        var url = "http://127.0.0.1:5000/get_embedding";

        var jsonList = JsonSerializer.Serialize(dto);

        var content = new StringContent(jsonList,Encoding.UTF8, "application/json");

        _httpClient.Timeout = TimeSpan.FromMinutes(30);
        var request = await _httpClient.PostAsync(url,content);

        request.EnsureSuccessStatusCode();

        List<VectorResponse> response = new List<VectorResponse>();

        try
        {
            response = JsonSerializer.Deserialize<List<VectorResponse>>(await request.Content.ReadAsStringAsync());
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        if (response.IsNullOrEmpty())
        {
            return Result.Failure(APIErrrors.ApiResponseError);
        }

        List<ShowInfo> showInfos = response.Select(x => new ShowInfo { Id = x.Id, ShowId = x.Id, VectorDouble = x.Embeddings }).ToList();

        await _databaseInserter.SaveVectorsToShowInfo(showInfos);



        return Result.Success();

    }
}
    
   