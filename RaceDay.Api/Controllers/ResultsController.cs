using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/results
        [HttpPost]
        public async Task<IActionResult> CreateResult([FromBody] CreateResultRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            if (role != "Organiser")
                return Forbid();

            var enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentId == request.EnrolmentId);

            if (enrolment == null)
                return NotFound("Enrolment not found.");

            if (enrolment.Event!.OrganiserId != userId.Value)
                return Forbid();

            // Check if result already exists
            var existingResult = await _context.Results
                .FirstOrDefaultAsync(r => r.EnrolmentId == request.EnrolmentId);

            if (existingResult != null)
                return Conflict("A result already exists for this enrolment.");

            var result = new Result
            {
                EnrolmentId = request.EnrolmentId,
                FinishTime = request.FinishTime,
                Position = request.Position,
                Status = request.Status ?? "Finished",
                Notes = request.Notes
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            var dto = new ResultDto
            {
                ResultId = result.ResultId,
                EnrolmentId = result.EnrolmentId,
                FinishTime = result.FinishTime,
                Position = result.Position,
                Status = result.Status,
                Notes = result.Notes
            };

            return Created("", dto);
        }

        // GET: api/results/mine
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            if (role != "Participant")
                return Forbid();

            var results = await _context.Results
                .Include(r => r.Enrolment)
                .Where(r => r.Enrolment!.ParticipantId == userId.Value)
                .Select(r => new ResultDto
                {
                    ResultId = r.ResultId,
                    EnrolmentId = r.EnrolmentId,
                    FinishTime = r.FinishTime,
                    Position = r.Position,
                    Status = r.Status,
                    Notes = r.Notes
                })
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/results/event/{eventId}
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetResultsByEvent(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized("You must be logged in.");

            // Both roles can view results of an event, but Organiser is the main one
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
                return NotFound("Event not found.");

            var results = await _context.Results
                .Include(r => r.Enrolment)
                .Where(r => r.Enrolment!.EventId == eventId)
                .Select(r => new ResultDto
                {
                    ResultId = r.ResultId,
                    EnrolmentId = r.EnrolmentId,
                    FinishTime = r.FinishTime,
                    Position = r.Position,
                    Status = r.Status,
                    Notes = r.Notes
                })
                .ToListAsync();

            return Ok(results);
        }
    }
}
