using System.Collections.Generic;
using System.Threading.Tasks;

namespace deaplom.Services
{
    public interface IBaseService<T>
    {
        Task<List<T>> GetAllAsync(int userId);
        Task<T?> GetByIdAsync(int id, int userId);
        Task<bool> AddAsync(T entity, int userId);
        Task<bool> UpdateAsync(T entity, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}