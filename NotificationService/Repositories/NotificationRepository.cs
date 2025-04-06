using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            notification.Id = Guid.NewGuid();
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsSent = false;

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification> UpdateSentStatusAsync(Guid id, bool isSent)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return null;

            notification.IsSent = isSent;
            notification.SentAt = isSent ? DateTime.UtcNow : null;
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<IEnumerable<Notification>> GetPendingNotificationsAsync()
        {
            return await _context.Notifications
                .Where(n => !n.IsSent)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}
