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

        // GET: api/Shows
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Show>>> GetShows()
        {
            return await _context.Shows.ToListAsync();
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

        // PUT: api/Shows/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShow(int id, Show show)
        {
            if (id != show.Id)
            {
                return BadRequest();
            }

            _context.Entry(show).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpGet("vector/{id}")]
        public async Task<IActionResult> GetShowVector(int id)
        {
            Show? show = await _context.FindAsync<Show>(id);
            if (show == null)
            {
                return NotFound();
            }

            var vector = show.VectorDouble;
            if(vector!= null)
            {
                return Ok(vector);
            }
            else
            {
                return StatusCode(500, "Error getting the vector from the api");
            }
        }

        [HttpGet("vectors/{id1}/{id2}/{id3}")]
        public async Task<List<int>>  GetRecomendedShows(int id1, int id2, int id3)
        {
            List<int> showIds = new List<int>() { id1, id2, id3 };
            List<double[]> showVectors = new List<double[]>();
           
            foreach (var id in showIds)
            {
                Show? show = await _context.FindAsync<Show>(id);
                double[] showVector = show.VectorDouble;
                showVectors.Add(showVector);

            }
            if(showVectors!= null)
            {
                double[] averageVector = VectorEngine.CalculateAverageVector(showVectors);
                List<Show> allShows =  _context.Shows.ToList();
                List<int> recomendedShowIds = VectorEngine.GetSimilarities(allShows, averageVector, 8);
                return recomendedShowIds;
            }

            else
            {
                return new List<int>();
            }
        }


        // POST: api/Shows
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Show>> PostShow(Show show)
        {
            _context.Shows.Add(show);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShow", new { id = show.Id }, show);
        }

        // DELETE: api/Shows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShow(int id)
        {
            var show = await _context.Shows.FindAsync(id);
            if (show == null)
            {
                return NotFound();
            }

            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShowExists(int id)
        {
            return _context.Shows.Any(e => e.Id == id);
        }
    }
}