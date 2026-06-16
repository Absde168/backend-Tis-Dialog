using deaplom.Model;
using Microsoft.EntityFrameworkCore;

namespace deaplom.Services
{
    public class SystemNotificationService : IBaseService<SystemNotification>
    {
        private readonly TisDialogContext _context;

        public SystemNotificationService(TisDialogContext context)
        {
            _context = context;
        }

        public async Task<List<SystemNotification>> GetAllAsync(int userId)
        {
            // Возвращаем глобальные уведомления и уведомления для конкретного пользователя
            return await _context.SystemNotifications
                .Where(sn => sn.UserId == null || sn.UserId == userId)
                .OrderByDescending(sn => sn.CreatedAt)
                .ToListAsync();
        }

        public async Task<SystemNotification?> GetByIdAsync(int id, int userId)
        {
            return await _context.SystemNotifications
                .FirstOrDefaultAsync(sn => sn.Id == id && (sn.UserId == null || sn.UserId == userId));
        }

        public async Task<bool> AddAsync(SystemNotification entity, int userId)
        {
            // Административная функция, можно ограничить
            // или оставить как есть, если добавляет система
            _context.SystemNotifications.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(SystemNotification entity, int userId)
        {
            // Обычно уведомления не редактируются
            return false;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var notification = await _context.SystemNotifications
                .FirstOrDefaultAsync(sn => sn.Id == id);

            if (notification == null) return false;

            _context.SystemNotifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}   