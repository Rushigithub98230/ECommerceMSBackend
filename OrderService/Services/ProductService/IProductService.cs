using EComMSSharedLibrary.Models;
using OrderService.Dtos;

namespace OrderService.Services.ProductService
{
    public interface IProductService
    {
        Task<ApiResponse<ProductDto>> GetProductAsync(Guid productId);
        Task<ApiResponse<bool>> CheckStockAvailabilityAsync(Guid productId, int quantity);
        Task<ApiResponse<bool>> DeductStockAsync(Guid productId, int quantity);
    }
}
