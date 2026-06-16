using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using deaplom.Model;

namespace deaplom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionHistoryController : ControllerBase
    {
        private readonly TisDialogContext _context;

        public ConnectionHistoryController(TisDialogContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConnectionHistory>>> GetHistory()
        {
            return await _context.ConnectionHistory.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<ConnectionHistory>> Create(ConnectionHistory item)
        {
            _context.ConnectionHistory.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHistory), new { id = item.Id }, item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ConnectionHistory.FindAsync(id);
            if (item == null) return NotFound();
            _context.ConnectionHistory.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}