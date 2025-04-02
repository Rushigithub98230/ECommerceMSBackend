using OrderService.Dtos;

namespace OrderService.Services.IService
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string customerId);
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto, string customerId);
        Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto statusDto);
        Task<bool> CancelOrderAsync(int id, string customerId);
    }
}
