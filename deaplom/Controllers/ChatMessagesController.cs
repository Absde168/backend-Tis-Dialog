using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using deaplom.Model;

namespace deaplom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatMessagesController : ControllerBase
    {
        private readonly TisDialogContext _context;

        public ChatMessagesController(TisDialogContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> Get()
        {
            return await _context.ChatMessages.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<ChatMessage>> Post(ChatMessage msg)
        {
            _context.ChatMessages.Add(msg);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = msg.Id }, msg);
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