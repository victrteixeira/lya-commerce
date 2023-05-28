using Commerce.Core.Entities;

namespace Commerce.Core.Interfaces;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IReadOnlyCollection<Product>?> GetByManufacturerAsync(string manufacturer);
    Task<IReadOnlyCollection<Product>> GetByCategoryAsync(string category);
    Task<IReadOnlyCollection<Product>?> GetBySubCategoryAsync(string subCategory);
}