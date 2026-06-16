using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using deaplom.Model;

namespace deaplom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentHistoryController : ControllerBase
    {
        private readonly TisDialogContext _context;

        public PaymentHistoryController(TisDialogContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentHistory>>> GetPayments()
        {
            return await _context.PaymentHistory.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<PaymentHistory>> Create(PaymentHistory item)
        {
            _context.PaymentHistory.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPayments), new { id = item.Id }, item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.PaymentHistory.FindAsync(id);
            if (item == null) return NotFound();
            _context.PaymentHistory.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}