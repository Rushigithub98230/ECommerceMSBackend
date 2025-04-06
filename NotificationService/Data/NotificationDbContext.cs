using Microsoft.EntityFrameworkCore;
using NotificationService.Models;
using System.Collections.Generic;

namespace NotificationService.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
    }
}
