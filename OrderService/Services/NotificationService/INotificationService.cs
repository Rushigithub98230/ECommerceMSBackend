using OrderService.Models;
using RabbitMQ.Client;
using System.Text;

namespace OrderService.Services.NotificationService
{
    public interface INotificationService
    {
        Task SendOrderCreatedNotificationAsync(Order order);
        Task SendOrderStatusChangedNotificationAsync(Order order);
    }


}
