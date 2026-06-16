using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using deaplom.Model;

namespace deaplom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SystemNotificationsController : ControllerBase
    {
        private readonly TisDialogContext _context;

        public SystemNotificationsController(TisDialogContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemNotification>>> GetNotifications()
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value ?? "0");
            if (userId == 0) return Unauthorized();

            var notifications = await _context.SystemNotifications
                .Where(n => n.UserId == null || n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPost]
        public async Task<ActionResult<SystemNotification>> CreateNotification(SystemNotification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            _context.SystemNotifications.Add(notification);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNotifications), new { id = notification.Id }, notification);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value ?? "0");
            if (userId == 0) return Unauthorized();

            var notification = await _context.SystemNotifications
                .FirstOrDefaultAsync(n => n.Id == id && (n.UserId == null || n.UserId == userId));

            if (notification == null) return NotFound();

            _context.SystemNotifications.Remove(notification);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
