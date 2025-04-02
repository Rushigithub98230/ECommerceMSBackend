using OrderService.Models;

namespace OrderService.Data.Repositories.IRepository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId);
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task<bool> OrderExistsAsync(int id);
    }
}
