using ProductService.Models;

namespace ProductService.Repositories.IRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsBySellerAsync(string sellerId);
        Task<Product> GetProductByIdAsync(int id);
        Task<bool> ProductExistsAsync(int id);
        Task<bool> IsInStockAsync(int id, int quantity);
        Task<bool> DecrementStockAsync(int id, int quantity);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    }
}
