using Microsoft.EntityFrameworkCore;
using NotificationServiceNew.Models;

namespace NotificationServiceNew.Data
{
     public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
    }
}
