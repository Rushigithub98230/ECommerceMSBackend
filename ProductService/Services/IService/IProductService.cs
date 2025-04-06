using EComMSSharedLibrary.Models;
using ProductService.DTos;

namespace ProductService.Services.IService
{
    public interface IProductService
    {
        Task<ApiResponse<ProductDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ProductDto>>> GetBySellerIdAsync(Guid sellerId);
        Task<ApiResponse<IEnumerable<ProductDto>>> GetByCategoryIdAsync(Guid categoryId);
        Task<ApiResponse<ProductDto>> CreateAsync(Guid sellerId, CreateProductDto productDto);
        Task<ApiResponse<ProductDto>> UpdateAsync(Guid id, Guid sellerId, UpdateProductDto productDto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id, Guid sellerId);
        Task<ApiResponse<bool>> UpdateStockAsync(Guid id, UpdateStockDto updateStockDto);
        Task<ApiResponse<bool>> CheckStockAvailabilityAsync(Guid id, int quantity);
    }
}
