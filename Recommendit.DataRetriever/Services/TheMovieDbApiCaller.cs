using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using DataRetriever.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Recommendit.Result;

namespace DataRetriever.Services;

public class TheMovieDbApiCaller : ITheMovieDbApiCaller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TheMovieDbApiCaller> _logger;

    public TheMovieDbApiCaller(HttpClient httpClient, IConfiguration configuration, ILogger<TheMovieDbApiCaller> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<List<ShowData>>> GetApiResponse(string bearerToken,string pageNumber)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add($"Authorization", $"Bearer {bearerToken}");
        List<ShowData> responseContent = new List<ShowData>() { };
        
        string apiUrl = $"{_configuration["MovieDb:ApiUrl"]}series?page=" + pageNumber;
        _logger.LogInformation(apiUrl);

        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

        var responseResult = VerifyValidResponse(response);

        if (responseResult.Result.IsFailure)
        {
            return Result<List<ShowData>>.Failure(responseResult.Result.Error);
        }
        responseContent = responseResult.Result.Value;

        return Result<List<ShowData>>.Success(responseContent);


    }

    private async Task<Result<List<ShowData>>> VerifyValidResponse(HttpResponseMessage response)
    {

        if (!response.IsSuccessStatusCode)
        {
            return Result<List<ShowData>>.Failure(APIErrrors.ApiResponseError);
        }

        string content = await response.Content.ReadAsStringAsync();

        var isSuccess = JsonSerializer.Deserialize<ApiResponse>(content);

        if (isSuccess != null && isSuccess.Status == "success")
        {
            return Result<List<ShowData>>.Success(isSuccess.Data);
        }

        return Result<List<ShowData>>.Failure(APIErrrors.ApiResponseError);
    }
    
    public async Task<string?> PostLogin(string apikey, string apiUrl)
    {
        var login = new Login()
        {
            apikey = apikey,
        };

        var json = JsonSerializer.Serialize(login);
        var content = new StringContent(json,Encoding.UTF8, "application/json");

        var url = $"{apiUrl}login";
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("accept","application/json");
        
        var response = await _httpClient.PostAsync(requestUri:url , content:content );
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent,options);
        
            if (loginResponse != null && loginResponse.Data != null)
            {
                return loginResponse.Data.Token;
            }
        }
        else
        {
            _logger.LogError($"Error: {response.StatusCode}");
            _logger.LogError("Could not retrieve the login info", await response.Content.ReadAsStringAsync());
        }

        return null;
    }

 

  
}

internal class Login 
{
    public required string apikey { get; set; }
}


internal class LoginResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    
    [JsonPropertyName("data")]
    public LoginData Data { get; set; }
}

internal class LoginData
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}

    
    
   