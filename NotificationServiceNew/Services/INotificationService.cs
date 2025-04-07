using NotificationServiceNew.Dtos;

namespace NotificationServiceNew.Services
{
    public interface INotificationService
    {
        Task ProcessOrderCreatedMessageAsync(OrderCreatedMessage message);
        Task ProcessOrderStatusChangedMessageAsync(OrderStatusChangedMessage message);
    }
}
