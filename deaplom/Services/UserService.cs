using deaplom.Model;
using Microsoft.EntityFrameworkCore;

namespace deaplom.Services
{
    public class UserService : IBaseService<User>
    {
        private readonly TisDialogContext _context;

        public UserService(TisDialogContext context)
        {
            _context = context;
        }

        // В сервисе User логика немного отличается: он работает с конкретным пользователем (по ID)
        // и, например, не поддерживает "GetAllAsync" для всех пользователей.
        // Поэтому можно не реализовывать методы, которые не подходят.
        public async Task<List<User>> GetAllAsync(int userId)
        {
            // Возвращаем только профиль текущего пользователя
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return new List<User>();
            return new List<User> { user };
        }

        public async Task<User?> GetByIdAsync(int id, int userId)
        {
            if (id != userId) return null; // Пользователь может получить только свой профиль
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> AddAsync(User entity, int userId)
        {
            // Обычно регистрацию делает не сам пользователь, а администратор или через отдельный endpoint
            // Здесь просто добавляем в БД, если нужно
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(User entity, int userId)
        {
            if (entity.Id != userId) return false; // Пользователь может обновить только свой профиль

            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null) return false;

            existingUser.FirstName = entity.FirstName;
            existingUser.LastName = entity.LastName;
            existingUser.Email = entity.Email;
            existingUser.Phone = entity.Phone;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            if (id != userId) return false; // Пользователь может удалить только свой аккаунт

            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}