using Microsoft.EntityFrameworkCore;
using OrderService.Data.Context;
using OrderService.Data.Repositories.IRepository;
using OrderService.Models;

namespace OrderService.Data.Repositories.Repository
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly OrderDbContext _context;

        public OrderItemRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<OrderItem> GetOrderItemByIdAsync(int id)
        {
            return await _context.OrderItems.FindAsync(id);
        }

        public async Task AddOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderItemAsync(OrderItem orderItem)
        {
            _context.Entry(orderItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderItemAsync(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}
