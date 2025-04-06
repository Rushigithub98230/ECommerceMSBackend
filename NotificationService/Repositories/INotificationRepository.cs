using NotificationService.Models;

namespace NotificationService.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> CreateAsync(Notification notification);
        Task<Notification> UpdateSentStatusAsync(Guid id, bool isSent);
        Task<IEnumerable<Notification>> GetPendingNotificationsAsync();
    }
}
