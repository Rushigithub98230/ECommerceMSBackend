using OrderService.Models;

namespace OrderService.Data.DataAccessRepositories
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<Order>> GetBySellerIdAsync(Guid sellerId);
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateStatusAsync(Guid id, string status);
    }
}
