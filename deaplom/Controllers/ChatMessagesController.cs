using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using deaplom.Model;
using deaplom.Services;

namespace deaplom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatMessagesController : ControllerBase
    {
        private readonly TisDialogContext _context;
        private readonly GeminiService _gemini;

        public ChatMessagesController(TisDialogContext context, GeminiService gemini)
        {
            _context = context;
            _gemini = gemini;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> Get([FromQuery] int? userId)
        {
            var query = _context.ChatMessages.AsQueryable();
            if (userId.HasValue)
                query = query.Where(m => m.UserId == userId.Value);
            return await query.OrderBy(m => m.Timestamp).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<object>> Post(ChatMessage msg)
        {
            msg.Timestamp = DateTime.UtcNow;
            _context.ChatMessages.Add(msg);
            await _context.SaveChangesAsync();

            var aiText = await _gemini.GetReplyAsync(msg.Content);
            var botMsg = new ChatMessage
            {
                UserId     = msg.UserId,
                Content    = aiText,
                SenderType = "Support",
                Timestamp  = DateTime.UtcNow
            };
            _context.ChatMessages.Add(botMsg);
            await _context.SaveChangesAsync();

            return Ok(new List<ChatMessage> { msg, botMsg });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var msg = await _context.ChatMessages.FindAsync(id);
            if (msg == null) return NotFound();
            _context.ChatMessages.Remove(msg);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
