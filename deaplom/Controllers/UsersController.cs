using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using deaplom.Model;
using deaplom.Helpers;
using System.Security.Claims;

namespace deaplom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly TisDialogContext _context;

        public UsersController(TisDialogContext context) => _context = context;

        [HttpGet("profile")]
        public async Task<ActionResult<User>> GetProfile()
        {
            var userIdClaim = User.FindFirst("Id")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                var authenticatedUser = await _context.Users.FindAsync(userId);
                if (authenticatedUser != null)
                    return Ok(authenticatedUser);
            }

            var user = await _context.Users.FirstOrDefaultAsync();
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var userIdClaim = User.FindFirst("Id")?.Value;
            if (!int.TryParse(userIdClaim, out int requestingUserId) || requestingUserId != id)
                return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.Phone = request.Phone ?? user.Phone;
            user.Email = request.Email ?? user.Email;

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            user.PasswordHash = PasswordHelper.Hash(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class UpdateUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
