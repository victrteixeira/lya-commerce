using Commerce.Core.Entities;
using Commerce.Core.Interfaces;
using Commerce.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    private readonly CommerceContext _context;
    
    public ProductRepository(CommerceContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Product>?> GetByManufacturerAsync(string manufacturer)
    {
        return await _context.Products.AsNoTracking().Where(x => x.Manufacturer == manufacturer).ToListAsync();
    }

    public async Task<IReadOnlyCollection<Product>> GetByCategoryAsync(string category)
    {
        return await _context.Products.AsNoTracking().Where(x => x.Category == category).ToListAsync();
    }

    public async Task<IReadOnlyCollection<Product>?> GetBySubCategoryAsync(string subCategory)
    {
        return await _context.Products.AsNoTracking().Where(x => x.SubCategory == subCategory).ToListAsync();
    }
}