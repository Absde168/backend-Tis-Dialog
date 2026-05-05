using deaplom.Model;
using Microsoft.EntityFrameworkCore;

namespace deaplom.Services
{
    public class ChatMessageService : IBaseService<ChatMessage>
    {
        private readonly TisDialogContext _context;

        public ChatMessageService(TisDialogContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>> GetAllAsync(int userId)
        {
            return await _context.ChatMessages
                .Where(cm => cm.UserId == userId)
                .OrderBy(cm => cm.Timestamp)
                .ToListAsync();
        }

        public async Task<ChatMessage?> GetByIdAsync(int id, int userId)
        {
            return await _context.ChatMessages
                .FirstOrDefaultAsync(cm => cm.Id == id && cm.UserId == userId);
        }

        public async Task<bool> AddAsync(ChatMessage entity, int userId)
        {
            if (entity.UserId != userId) return false;
            entity.UserId = userId;
            _context.ChatMessages.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(ChatMessage entity, int userId)
        {
            // Обычно сообщения не редактируются
            return false;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var message = await _context.ChatMessages
                .FirstOrDefaultAsync(cm => cm.Id == id && cm.UserId == userId);

            if (message == null) return false;

            _context.ChatMessages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}