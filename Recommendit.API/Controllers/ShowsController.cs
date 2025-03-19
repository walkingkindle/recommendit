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
        //Returns an exact match of records found by name, otherwise returns any .Contains match.
        public async Task<IActionResult> GetRecordsByInput(string input)
        {

            var matchingRecords = await _showService.GetShowsExactMatchingRecordsAsync(input);

            if(matchingRecords.IsFailure)
            {
                matchingRecords = await _showService.GetMatchingShowRecordsAsync(input);
            };


            return Ok(matchingRecords);                
     
        }


        [HttpGet("suggest/{id1}/{id2}/{id3}")]
        // Uses cosine similarity to recommend shows based on user's preference.
        public async Task<IResult> GetRecommendedShows(int id1, int id2, int id3)
        {
            var recommendedShows = _showService.GetRecommendedShowsWithCosineSimilarity(new List<int> { id1, id2, id3 }, 5);


            return recommendedShows.Result.Match(
                onSuccess: () => Results.Ok(recommendedShows.Result.Value),
                onFailure: error => Results.BadRequest(recommendedShows.Result.Error));

        }
    }
}
