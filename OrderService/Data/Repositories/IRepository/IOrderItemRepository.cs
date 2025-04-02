using OrderService.Models;

namespace OrderService.Data.Repositories.IRepository
{
    public interface IOrderItemRepository
    {
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<OrderItem> GetOrderItemByIdAsync(int id);
        Task AddOrderItemAsync(OrderItem orderItem);
        Task UpdateOrderItemAsync(OrderItem orderItem);
        Task DeleteOrderItemAsync(int id);
    }
}
