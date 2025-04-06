using NotificationService.Dtos;

namespace NotificationService.Services
{
    public interface INotificationService
    {
        Task ProcessOrderCreatedMessageAsync(OrderCreatedMessage message);
        Task ProcessOrderStatusChangedMessageAsync(OrderStatusChangedMessage message);
    }
}
