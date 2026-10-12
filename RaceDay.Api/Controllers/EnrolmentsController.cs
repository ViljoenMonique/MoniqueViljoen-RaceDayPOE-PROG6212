using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/enrolments
        [HttpPost]
        public async Task<IActionResult> CreateEnrolment([FromBody] CreateEnrolmentRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            if (role != "Participant")
                return Forbid();

            var eventItem = await _context.Events.FindAsync(request.EventId);
            if (eventItem == null)
                return NotFound("Event not found.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId && c.EventId == request.EventId);

            if (category == null)
                return BadRequest("Invalid category for this event.");

            // Check if already enrolled
            var existing = await _context.Enrolments
                .FirstOrDefaultAsync(e => e.ParticipantId == userId.Value && e.EventId == request.EventId);

            if (existing != null)
                return Conflict("You are already enrolled in this event.");

            var enrolment = new Enrolment
            {
                ParticipantId = userId.Value,
                EventId = request.EventId,
                CategoryId = request.CategoryId,
                EnrolmentDate = DateTime.UtcNow,
                Status = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            var dto = new EnrolmentDto
            {
                EnrolmentId = enrolment.EnrolmentId,
                ParticipantId = enrolment.ParticipantId,
                EventId = enrolment.EventId,
                CategoryId = enrolment.CategoryId,
                EnrolmentDate = enrolment.EnrolmentDate,
                Status = enrolment.Status
            };

            return Created("", dto);
        }

        // GET: api/enrolments/mine
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            if (role != "Participant")
                return Forbid();

            var enrolments = await _context.Enrolments
                .Where(e => e.ParticipantId == userId.Value)
                .Select(e => new EnrolmentDto
                {
                    EnrolmentId = e.EnrolmentId,
                    ParticipantId = e.ParticipantId,
                    EventId = e.EventId,
                    CategoryId = e.CategoryId,
                    EnrolmentDate = e.EnrolmentDate,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // GET: api/enrolments/event/{eventId}
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetEnrolmentsByEvent(int eventId)
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

            var enrolments = await _context.Enrolments
                .Where(e => e.EventId == eventId)
                .Select(e => new EnrolmentDto
                {
                    EnrolmentId = e.EnrolmentId,
                    ParticipantId = e.ParticipantId,
                    EventId = e.EventId,
                    CategoryId = e.CategoryId,
                    EnrolmentDate = e.EnrolmentDate,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }
    }
}
