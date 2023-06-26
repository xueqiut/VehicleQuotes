using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleQuotes.Models;

namespace VehicleQuotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakesController : ControllerBase
    {
        private readonly VehicleQuotesContext _context;

        public MakesController(VehicleQuotesContext context)
        {
            _context = context;
        }

        // GET: api/Makes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Make>>> GetMakes()
        {
          if (_context.Makes == null)
          {
              return NotFound();
          }
            return await _context.Makes.ToListAsync();
        }

        // GET: api/Makes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Make>> GetMake(int id)
        {
          if (_context.Makes == null)
          {
              return NotFound();
          }
            var make = await _context.Makes.FindAsync(id);

            if (make == null)
            {
                return NotFound();
            }

            return make;
        }

        // PUT: api/Makes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMake(int id, Make make)
        {
            if (id != make.ID)
            {
                return BadRequest();
            }

            _context.Entry(make).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MakeExists(id))
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

        // POST: api/Makes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Make>> PostMake(Make make)
        {
          if (_context.Makes == null)
          {
              return Problem("Entity set 'VehicleQuotesContext.Makes'  is null.");
          }
            _context.Makes.Add(make);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMake", new { id = make.ID }, make);
        }

        // DELETE: api/Makes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMake(int id)
        {
            if (_context.Makes == null)
            {
                return NotFound();
            }
            var make = await _context.Makes.FindAsync(id);
            if (make == null)
            {
                return NotFound();
            }

            _context.Makes.Remove(make);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MakeExists(int id)
        {
            return (_context.Makes?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
