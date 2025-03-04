using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using DataRetriever.Models;
using Recommendit.Result;
using ShowPulse.Models;

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

    public async Task<Result<List<ShowDataDto>>> GetApiResponse(string bearerToken,string pageNumber)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add($"Authorization", $"Bearer {bearerToken}");
        List<ShowDataDto> responseContent = new List<ShowDataDto>() { };
        
        string apiUrl = $"{_configuration["Credentials:BaseUrl"]}?page=" + pageNumber;
        _logger.LogInformation(apiUrl);

        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

        var responseResult = VerifyValidResponse(response);

        if (responseResult.Result.IsFailure)
        {
            return Result<List<ShowDataDto>>.Failure(responseResult.Result.Error);
        }
        responseContent = responseResult.Result.Value;

        return Result<List<ShowDataDto>>.Success(responseContent);


    }

    private async Task<Result<List<ShowDataDto>>> VerifyValidResponse(HttpResponseMessage response)
    {

        if (!response.IsSuccessStatusCode)
        {
            return Result<List<ShowDataDto>>.Failure(APIErrrors.ApiResponseError);
        }

        var isSuccess = JsonSerializer.Deserialize<ApiResponse>(await response.Content.ReadAsStringAsync());

        if (isSuccess != null && isSuccess.Status == "success")
        {
            return Result<List<ShowDataDto>>.Success(isSuccess.Data);
        }

        return Result<List<ShowDataDto>>.Failure(APIErrrors.ApiResponseError);
    }
    
    public async Task<string?> PostLogin(string apikey, string apiUrl)
    {
        var login = new Login()
        {
            apikey = apikey,
        };

        var json = JsonSerializer.Serialize(login);
        var content = new StringContent(json,Encoding.UTF8, "application/json");
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("accept","application/json");
        
        var response = await _httpClient.PostAsync(requestUri: apiUrl, content:content );
        
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
        }

        return null; // Return null if unsuccessful
    }

 

  
}

internal class Login 
{
    public required string apikey { get; set; }
    public string? pin { get; set; }
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

    
    
   