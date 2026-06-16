using deaplom.Model;
using Microsoft.EntityFrameworkCore;

namespace deaplom.Services
{
    public class ConnectionHistoryService : IBaseService<ConnectionHistory>
    {
        private readonly TisDialogContext _context;

        public ConnectionHistoryService(TisDialogContext context)
        {
            _context = context;
        }

        public async Task<List<ConnectionHistory>> GetAllAsync(int userId)
        {
            return await _context.ConnectionHistory
                .Where(ch => ch.UserId == userId)
                .ToListAsync();
        }

        public async Task<ConnectionHistory?> GetByIdAsync(int id, int userId)
        {
            return await _context.ConnectionHistory
                .FirstOrDefaultAsync(ch => ch.Id == id && ch.UserId == userId);
        }

        public async Task<bool> AddAsync(ConnectionHistory entity, int userId)
        {
            if (entity.UserId != userId) return false; // Только для текущего пользователя
            entity.UserId = userId;
            _context.ConnectionHistory.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(ConnectionHistory entity, int userId)
        {
            // Обычно история подключений не редактируется
            return false;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var history = await _context.ConnectionHistory
                .FirstOrDefaultAsync(ch => ch.Id == id && ch.UserId == userId);

            if (history == null) return false;

            _context.ConnectionHistory.Remove(history);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}