using NotificationServiceNew.Models;

namespace NotificationServiceNew.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> CreateAsync(Notification notification);
        Task<Notification> UpdateSentStatusAsync(Guid id, bool isSent);
        Task<IEnumerable<Notification>> GetPendingNotificationsAsync();
    }
}
