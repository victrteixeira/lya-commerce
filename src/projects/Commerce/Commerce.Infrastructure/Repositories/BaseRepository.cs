using Commerce.Core.Entities;
using Commerce.Core.Interfaces;
using Commerce.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : Base
{
    private readonly CommerceContext _context;

    public BaseRepository(CommerceContext context)
    {
        _context = context;
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<T?> UpdateAsync(T entity, long id)
    {
        var oldEntity = await GetByIdAsync(id);
        if (oldEntity is null) return null;
        _context.Entry(oldEntity).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> RemoveAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null) return false;
        _context.Set<T>().Remove(entity);
        return await _context.SaveChangesAsync() != 0;
        
    }

    public virtual async Task<T?> GetByIdAsync(long id) => await _context.Set<T>().FindAsync(id) ?? null;

    public virtual async Task<IReadOnlyCollection<T>> GetAllAsync() => await _context.Set<T>().AsNoTracking().ToListAsync();
}