using deaplom.Model;
using Microsoft.EntityFrameworkCore;

namespace deaplom.Services
{
    public class PaymentHistoryService : IBaseService<PaymentHistory>
    {
        private readonly TisDialogContext _context;

        public PaymentHistoryService(TisDialogContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentHistory>> GetAllAsync(int userId)
        {
            return await _context.PaymentHistory
                .Where(ph => ph.UserId == userId)
                .ToListAsync();
        }

        public async Task<PaymentHistory?> GetByIdAsync(int id, int userId)
        {
            return await _context.PaymentHistory
                .FirstOrDefaultAsync(ph => ph.Id == id && ph.UserId == userId);
        }

        public async Task<bool> AddAsync(PaymentHistory entity, int userId)
        {
            if (entity.UserId != userId) return false;
            entity.UserId = userId;
            _context.PaymentHistory.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(PaymentHistory entity, int userId)
        {
   
            return false;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var payment = await _context.PaymentHistory
                .FirstOrDefaultAsync(ph => ph.Id == id && ph.UserId == userId);

            if (payment == null) return false;

            _context.PaymentHistory.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}