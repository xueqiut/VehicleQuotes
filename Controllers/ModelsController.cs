using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleQuotes.Models;
using VehicleQuotes.ResourceModels;

namespace VehicleQuotes.Controllers
{
    [Route(template: "api/Makes/{makeId}/[controller]/")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly VehicleQuotesContext _context;

        public ModelsController(VehicleQuotesContext context)
        {
            _context = context;
        }

        // GET: api/Models
/*         [HttpGet]
        public async Task<ActionResult<IEnumerable<Model>>> GetModels([FromRoute] int makeId)
        {
            Make? make = await _context.Makes.FindAsync(keyValues: makeId);

            if (make == null || _context.Models == null)
            {
                return NotFound();
            }
            return await _context.Models.Where(predicate: m => m.MakeID == makeId).ToListAsync();
        }
 */

        // GET: api/Models
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelSpecification>>> GetModels([FromRoute] int makeId)
        {
            Make? make = await _context.Makes.FindAsync(keyValues: makeId);

            if (make == null)
            {
                return NotFound();
            }
            
            var modelsToReturn = _context.Models
                .Where(m => m.MakeID == makeId)
                .Select(m => new ModelSpecification {
                    ID = m.ID,
                    Name = m.Name,
                    Styles = m.ModelStyles.Select(ms => new ModelSpecificationStyle {
                        BodyType = ms.BodyType.Name,
                        Size = ms.Size.Name,
                        Years = ms.ModelStyleYears.Select(msy => msy.Year).ToArray()
                    }).ToArray()
                });

            //Ling use deferred execution. The query is not executed until the result is iterated over or converted to a list.
            // https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/query-execution
            return await modelsToReturn.ToListAsync();
        }
        // GET: api/Models/5
/*         [HttpGet(template: "{id}")]
        public async Task<ActionResult<Model>> GetModel([FromRoute] int makeId, int id)
        {
            if (_context.Models == null)
            {
                return NotFound();
            }
            var model = await _context.Models.Where(predicate: m => m.MakeID == makeId && m.ID == id).FirstOrDefaultAsync();

            if (model == null)
            {
                return NotFound();
            }

            return model;
        } */

        [HttpGet(template: "{id}")]
        public async Task<ActionResult<ModelSpecification>> GetModel ([FromRoute] int makeId, int id) {
            // Look for the model specified by the given identifiers and also load
            // all related data that we care about for this method.
            // Include() use "join" QSL Clause to eagerly load related data. Instead of lazy loading which would make a separate query for each related entity.
            // https://stackoverflow.com/questions/7370555/the-purpose-of-include-in-asp-net-mvc-entity-framework
            var model = await _context.Models
                .Include("ModelStyles.BodyType")
                .Include("ModelStyles.Size")
                .Include("ModelStyles.ModelStyleYears")
                .FirstOrDefaultAsync(m => m.MakeID == makeId && m.ID == id);

            // If we couldn't find it, respond with a 404.
            if (model == null)
            {
                return NotFound();
            }

            // Use the fetched data to construct a `ModelSpecification` to use in the response.
            return new ModelSpecification {
                ID = model.ID,
                Name = model.Name,
                Styles = model.ModelStyles.Select(ms => new ModelSpecificationStyle {
                    BodyType = ms.BodyType.Name,
                    Size = ms.Size.Name,
                    Years = ms.ModelStyleYears.Select(msy => msy.Year).ToArray()
                }).ToArray()
            };
        }

        // PUT: api/Models/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    /*     [HttpPut("{id}")]
        public async Task<IActionResult> PutModel(int id, Model model)
        {
            if (id != model.ID)
            {
                return BadRequest();
            }

            _context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        } */

        [HttpPut("{id}")]
        // Expect `makeId` and `id` from the URL and a `ModelSpecification` from the request payload.
        public async Task<IActionResult> PutModel([FromRoute] int makeId, int id, ModelSpecification model)
        {
            // If the id in the URL and the request payload are different, return a 400.
            if (id != model.ID)
            {
                return BadRequest();
            }

            // Obtain the `models` record that we want to update. Include any related
            // data that we want to update as well.
            var modelToUpdate = await _context.Models
                .Include(m => m.ModelStyles)
                .FirstOrDefaultAsync(m => m.MakeID == makeId && m.ID == id);

            // If we can't find the record, then return a 404.
            if (modelToUpdate == null)
            {
                return NotFound();
            }

            // Update the record with what came in the request payload.
            modelToUpdate.Name = model.Name;

            // Build EF Core entities based on the incoming Resource Model object.
            modelToUpdate.ModelStyles = model.Styles.Select(style => new ModelStyle {

                // The single method will throw an exception if the record isn't found.
                // If we wanted not-founds to return null, we could have used SingleOrDefault instead.
                BodyType = _context.BodyTypes.Single(bodyType => bodyType.Name == style.BodyType),
                Size = _context.Sizes.SingleOrDefault(size => size.Name == style.Size),

                ModelStyleYears = style.Years.Select(year => new ModelStyleYear {
                    Year = year
                }).ToList()
            }).ToList();

            try
            {
                // Try saving the changes. This will run the UPDATE statement in the database.
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                // If there's an error updating, respond accordingly.
                return Conflict();
            }

            // Finally return a 204 if everything went well.
            return NoContent();
        }

        // POST: api/Models
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
 /*        [HttpPost]
        public async Task<ActionResult<Model>> PostModel(Model model)
        {
          if (_context.Models == null)
          {
              return Problem("Entity set 'VehicleQuotesContext.Models'  is null.");
          }
            _context.Models.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModel", new { id = model.ID }, model);
        } */

        [HttpPost]
        // Return a `ModelSpecification`s and expect `makeId` from the URL and a `ModelSpecification` from the request payload.
        public async Task<ActionResult<ModelSpecification>> PostModel([FromRoute] int makeId, ModelSpecification model)
        {
            // First, try to find the make specified by the incoming `makeId`.
            var make = await _context.Makes.FindAsync(makeId);

            // Respond with 404 if not found.
            if (make == null)
            {
                return NotFound();
            }

            // Build out a new `Model` entity, complete with all related data, based on
            // the `ModelSpecification` parameter.
            var modelToCreate = new Model {
                Make = make,
                Name = model.Name,

                ModelStyles = model.Styles.Select(style => new ModelStyle {
                    // Notice how we search both body type and size by their name field.
                    // We can do that because their names are unique.
                    BodyType = _context.BodyTypes.Single(bodyType => bodyType.Name == style.BodyType),
                    Size = _context.Sizes.Single(size => size.Name == style.Size),

                    ModelStyleYears = style.Years.Select(year => new ModelStyleYear {
                        Year = year
                    }).ToArray()
                }).ToArray()
            };

            // Add it to the DbContext.
            _context.Models.Add(modelToCreate);

            try
            {
                // Try running the INSERTs.
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                // Return accordingly if an error happens.
                return Conflict();
            }

            // Get back the autogenerated ID of the record we just INSERTed.
            model.ID = modelToCreate.ID;

            // Finally, return a 201 including a location header containing the newly
            // created resource's URL and the resource itself in the response payload.
            return CreatedAtAction(
                nameof(GetModel),
                new { makeId = makeId, id = model.ID },
                model
            );
        }

        // DELETE: api/Models/5
        [HttpDelete("{id}")]
        // Expect `makeId` and `id` from the URL.
        public async Task<IActionResult> DeleteModel([FromRoute] int makeId, int id)
        {
            // Try to find the record identified by the ids from the URL.
            var model = await _context.Models.FirstOrDefaultAsync(m => m.MakeID == makeId && m.ID == id);

            // Respond with a 404 if we can't find it.
            if (model == null)
            {
                return NotFound();
            }

            // Mark the entity for removal and run the DELETE.
            _context.Models.Remove(model);
            await _context.SaveChangesAsync();

            // Respond with a 204.
            return NoContent();
        }

        private bool ModelExists(int id)
        {
            return (_context.Models?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
