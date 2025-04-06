using ProductService.DTos;

namespace ProductService.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductDto>> GetProductsBySellerAsync(string sellerId);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> AddProductAsync(CreateProductDto productDto, string sellerId);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto productDto, string sellerId);
        Task DeleteProductAsync(int id, string? sellerId);
        Task<bool> IsInStockAsync(int id, int quantity);
        Task<bool> DecrementStockAsync(int id, int quantity);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    }
}
