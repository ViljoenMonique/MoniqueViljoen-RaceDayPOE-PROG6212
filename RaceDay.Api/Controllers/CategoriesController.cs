using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/categories/event/{eventId}
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetCategoriesByEvent(int eventId)
        {
            var categories = await _context.Categories
                .Where(c => c.EventId == eventId)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    EventId = c.EventId,
                    CategoryName = c.CategoryName,
                    DistanceKm = c.DistanceKm,
                    AgeMin = c.AgeMin,
                    AgeMax = c.AgeMax,
                    GenderRestriction = c.GenderRestriction,
                    EntryFee = c.EntryFee
                })
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/categories/event/{eventId}
        [HttpPost("event/{eventId}")]
        public async Task<IActionResult> CreateCategory(int eventId, [FromBody] CreateCategoryRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            if (role != "Organiser")
                return Forbid();

            var eventItem = await _context.Events.FindAsync(eventId);

            if (eventItem == null)
                return NotFound("Event not found.");

            if (eventItem.OrganiserId != userId.Value)
                return Forbid();

            if (string.IsNullOrWhiteSpace(request.CategoryName))
                return BadRequest("CategoryName is required.");

            var category = new Category
            {
                EventId = eventId,
                CategoryName = request.CategoryName,
                DistanceKm = request.DistanceKm,
                AgeMin = request.AgeMin,
                AgeMax = request.AgeMax,
                GenderRestriction = request.GenderRestriction,
                EntryFee = request.EntryFee
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var dto = new CategoryDto
            {
                CategoryId = category.CategoryId,
                EventId = category.EventId,
                CategoryName = category.CategoryName,
                DistanceKm = category.DistanceKm,
                AgeMin = category.AgeMin,
                AgeMax = category.AgeMax,
                GenderRestriction = category.GenderRestriction,
                EntryFee = category.EntryFee
            };

            return Created("", dto);
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            if (role != "Organiser")
                return Forbid();

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                return NotFound("Category not found.");

            if (category.Event!.OrganiserId != userId.Value)
                return Forbid();

            category.CategoryName = request.CategoryName;
            category.DistanceKm = request.DistanceKm;
            category.AgeMin = request.AgeMin;
            category.AgeMax = request.AgeMax;
            category.GenderRestriction = request.GenderRestriction;
            category.EntryFee = request.EntryFee;

            await _context.SaveChangesAsync();

            return Ok(new CategoryDto
            {
                CategoryId = category.CategoryId,
                EventId = category.EventId,
                CategoryName = category.CategoryName,
                DistanceKm = category.DistanceKm,
                AgeMin = category.AgeMin,
                AgeMax = category.AgeMax,
                GenderRestriction = category.GenderRestriction,
                EntryFee = category.EntryFee
            });
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            if (role != "Organiser")
                return Forbid();

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                return NotFound("Category not found.");

            if (category.Event!.OrganiserId != userId.Value)
                return Forbid();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Category deleted successfully." });
        }
    }
}
