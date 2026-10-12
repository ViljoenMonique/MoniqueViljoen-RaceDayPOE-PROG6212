using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _context.Events
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    OrganiserId = e.OrganiserId,
                    EventName = e.EventName,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    DistanceKm = e.DistanceKm,
                    EventType = e.EventType,
                    MaxParticipants = e.MaxParticipants,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
                return NotFound("Event not found.");

            var dto = new EventDto
            {
                EventId = eventItem.EventId,
                OrganiserId = eventItem.OrganiserId,
                EventName = eventItem.EventName,
                Description = eventItem.Description,
                EventDate = eventItem.EventDate,
                Location = eventItem.Location,
                DistanceKm = eventItem.DistanceKm,
                EventType = eventItem.EventType,
                MaxParticipants = eventItem.MaxParticipants,
                Status = eventItem.Status
            };

            return Ok(dto);
        }

        // GET: api/events/mine
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyEvents()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            if (role != "Organiser")
                return Forbid();

            var events = await _context.Events
                .Where(e => e.OrganiserId == userId.Value)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    OrganiserId = e.OrganiserId,
                    EventName = e.EventName,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    DistanceKm = e.DistanceKm,
                    EventType = e.EventType,
                    MaxParticipants = e.MaxParticipants,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(events);
        }

        // POST: api/events
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            if (role != "Organiser")
                return Forbid();

            if (string.IsNullOrWhiteSpace(request.EventName) ||
                string.IsNullOrWhiteSpace(request.Location) ||
                string.IsNullOrWhiteSpace(request.EventType))
            {
                return BadRequest("EventName, Location and EventType are required.");
            }

            var newEvent = new Event
            {
                OrganiserId = userId.Value,
                EventName = request.EventName,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                DistanceKm = request.DistanceKm,
                EventType = request.EventType,
                MaxParticipants = request.MaxParticipants,
                Status = "Open"
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            var dto = new EventDto
            {
                EventId = newEvent.EventId,
                OrganiserId = newEvent.OrganiserId,
                EventName = newEvent.EventName,
                Description = newEvent.Description,
                EventDate = newEvent.EventDate,
                Location = newEvent.Location,
                DistanceKm = newEvent.DistanceKm,
                EventType = newEvent.EventType,
                MaxParticipants = newEvent.MaxParticipants,
                Status = newEvent.Status
            };

            return CreatedAtAction(nameof(GetEventById), new { id = newEvent.EventId }, dto);
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            if (role != "Organiser")
                return Forbid();

            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
                return NotFound("Event not found.");

            if (eventItem.OrganiserId != userId.Value)
                return Forbid();

            eventItem.EventName = request.EventName;
            eventItem.Description = request.Description;
            eventItem.EventDate = request.EventDate;
            eventItem.Location = request.Location;
            eventItem.DistanceKm = request.DistanceKm;
            eventItem.EventType = request.EventType;
            eventItem.MaxParticipants = request.MaxParticipants;
            eventItem.Status = request.Status;

            await _context.SaveChangesAsync();

            return Ok(new EventDto
            {
                EventId = eventItem.EventId,
                OrganiserId = eventItem.OrganiserId,
                EventName = eventItem.EventName,
                Description = eventItem.Description,
                EventDate = eventItem.EventDate,
                Location = eventItem.Location,
                DistanceKm = eventItem.DistanceKm,
                EventType = eventItem.EventType,
                MaxParticipants = eventItem.MaxParticipants,
                Status = eventItem.Status
            });
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            if (role != "Organiser")
                return Forbid();

            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
                return NotFound("Event not found.");

            if (eventItem.OrganiserId != userId.Value)
                return Forbid();

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Event deleted successfully." });
        }
    }
}
