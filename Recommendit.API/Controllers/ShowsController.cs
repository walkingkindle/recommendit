using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Recommendit.Result;
using Recommendit.Interface;

namespace ShowPulse.Controllers
{
    [ApiController]
    [Route("api/shows")]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }
        

        // GET: api/Shows/5
        [HttpGet("{id}")]
        public async Task<IResult> GetShow(int id)
        {
           var show = await _showService.GetShowByIdAsync(id);
           
            return show.Match(onSuccess: () => Results.Ok(show.Value), onFailure: (error) => Results.BadRequest(show.Error));

        }

        //GET: api/records
        [HttpGet("search/{input}")]
        public async Task<IActionResult> GetRecordsByInput(string input)
        {

            var matchingRecords = await _showService.GetMatchingShowRecordsAsync(input);
       

            return Ok(matchingRecords);                
     
        }


        [HttpPost("suggest")]
        public async Task<IResult> GetRecommendedShows(List<int> showIds, int topN=10)
        {
            var recommendedShows = _showService.GetRecommendedShowsWithCosineSimilarity(showIds, topN);


            return recommendedShows.Result.Match(
                onSuccess: () => Results.Ok(recommendedShows.Result.Value),
                onFailure: error => Results.BadRequest(recommendedShows.Result.Error));

        }
    }
}
