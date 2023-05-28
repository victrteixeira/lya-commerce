using Commerce.Core.Entities;

namespace Commerce.Core.Interfaces;

public interface IBaseRepository<T> where T : Base
{
    Task<T> AddAsync(T entity);
    Task<T?> UpdateAsync(T entity, long id);
    Task<bool> RemoveAsync(long id);
    Task<T?> GetByIdAsync(long id);
    Task<IReadOnlyCollection<T>> GetAllAsync();
}