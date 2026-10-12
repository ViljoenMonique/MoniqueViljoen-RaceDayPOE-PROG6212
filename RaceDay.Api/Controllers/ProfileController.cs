using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ProfileController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/profile/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("You must be logged in to view your profile.");
            }

            var user = await _context.Users.FindAsync(userId.Value);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var profile = new ProfileDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone
            };

            return Ok(profile);
        }

        // PUT: api/profile/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("You must be logged in to update your profile.");
            }

            var user = await _context.Users.FindAsync(userId.Value);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Phone = request.Phone;

            await _context.SaveChangesAsync();

            return Ok(new ProfileDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone
            });
        }
    }
}
