using ProductService.Models;

namespace ProductService.Data.DataAccessRepositories.ProductRepositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetBySellerIdAsync(Guid sellerId);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateStockAsync(Guid id, int quantity);
    }
}
