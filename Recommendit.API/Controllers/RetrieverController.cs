using DataRetriever.Models;
using Microsoft.AspNetCore.Mvc;
using Recommendit.DataRetriever.Models;
using Recommendit.Models;
using Recommendit.Result;

namespace ShowPulse.Controllers
{
    [ApiController]
    [Route("api/retriever")]
    public class RetrieverController : ControllerBase
    {
        private readonly IShowsRetriever _showsRetriever;
        private readonly IMongoDbService _mongoDbService;

        public RetrieverController(IShowsRetriever showsRetriever, IMongoDbService mongoDbService)
        {
            _showsRetriever = showsRetriever;
            _mongoDbService = mongoDbService;
        }

        [HttpGet("insert")]
        public async Task<IResult> InsertRecords()
        {
            var responseResult = await _showsRetriever.AddShows();

            if (!responseResult.IsSuccess)
            {
                return Results.BadRequest(responseResult.Error);
            }

            return Results.NoContent();


        }

        [HttpGet("login")]
        public async Task<IResult> Login()
        {
            var result = await _showsRetriever.PostLogin();
            return Results.Ok(result);
        }


        [HttpGet("getVectors")]
        public async Task<IResult> GetVectors()
        {
            await _mongoDbService.SetShowInfoVectorsAsync();

            return Results.Ok();
        }
        
        
        



    }
}

