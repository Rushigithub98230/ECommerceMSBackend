using EComMSSharedLibrary.Models;
using OrderService.Dtos;

namespace OrderService.Services
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<IEnumerable<OrderDto>>> GetByCustomerIdAsync(Guid customerId);
        Task<ApiResponse<IEnumerable<OrderDto>>> GetBySellerIdAsync(Guid sellerId);
        Task<ApiResponse<OrderDto>> CreateAsync(Guid customerId, CreateOrderDto orderDto);
        Task<ApiResponse<OrderDto>> UpdateStatusAsync(Guid id, UpdateOrderStatusDto updateStatusDto);
    }
}
