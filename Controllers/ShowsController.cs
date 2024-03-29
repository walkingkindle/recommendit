﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShowPulse.Models;
using System.Text.Json;
using System.Text;
using ShowPulse.Engine;

namespace ShowPulse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly ShowContext _context;

        public ShowsController(ShowContext context)
        {
            _context = context;
        }

        // GET: api/Shows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Show>> GetShow(int id)
        {
            var show = await _context.Shows.FindAsync(id);

            if (show == null)
            {
                return NotFound();
            }

            return show;
        }

        //GET: api/records
        [HttpGet("search/{input}")]
        //Returns an exact match of records found by name, otherwise returns any .Contains match.
        public ActionResult<IEnumerable<Show>> GetRecordsByInput(string input)
        {
            try
            {
                var exactmatchedRecords = _context.Shows.Where(s => s.Name == input)
                   .Select(s => new Show
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Description = s.Description,
                       ImageUrl = s.ImageUrl,
                       ReleaseYear = s.ReleaseYear,
                       FinalEpisodeAired = s.FinalEpisodeAired,
                       Score = s.Score,
                       OriginalCountry = s.OriginalCountry,
                       OriginalLanguage = s.OriginalLanguage
                   }).ToList(); ;//exact match?

                List<Show> matchedRecords = _context.Shows.Where(s => s.Name.Contains(input))
                    .Take(10).Select(s => new Show
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        ReleaseYear = s.ReleaseYear,
                        ImageUrl = s.ImageUrl
                    }).ToList();

                if (exactmatchedRecords.Count == 0)
                {
                    return Ok(matchedRecords);
                }
                return Ok(exactmatchedRecords);
            }
            catch (Exception ex)
            {
                 return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("suggest/{id1}/{id2}/{id3}")]
        // Uses cosine similarity to recommend shows based on user's preference.
        public async Task<IActionResult> GetRecommendedShows(int id1, int id2, int id3)
        {
            List<int> showIds = new List<int> { id1, id2, id3 };

            // Fetch all shows from the database asynchronously
            var allShows = await _context.Shows
                .Select(s => new { s.Id, s.VectorDouble })
                .ToListAsync();

            // Filter shows by the specified IDs
            var selectedShows = allShows.Where(b => showIds.Contains(b.Id)).ToList();

            if (selectedShows.Any())
            {
                // Calculate average vector
                List<double[]?> vectorList = selectedShows.Select(s => s.VectorDouble).ToList();
                double[] averageVector = VectorEngine.CalculateAverageVector(vectorList);

                // Calculate similarities and get recommended show IDs asynchronously
                List<int> recommendedShowIds = await VectorEngine.GetSimilarities(
                    allShows.Select(s => new ShowInfo { Id = s.Id, VectorDouble = s.VectorDouble }).ToList(),
                    averageVector,
                    8);

                return Ok(recommendedShowIds);
            }
            else
            {
                // Return the input IDs if no shows are found
                return NotFound("Not shows found for the specified IDs.");
            }
        }
    }
}
