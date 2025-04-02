using Microsoft.EntityFrameworkCore;
using UserService.Entity;

namespace UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Seed admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin123@yopmail.com.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = "admin",
                    FirstName = "Admin",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow,
                    PhoneNumber = "9823016913"
                },
                new User
                {
                    Id = 2,
                    Email = "seller123@yopmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Seller123!"),
                    Role = "seller",
                    FirstName = "Seller",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow,
                    PhoneNumber = "9823016913"
                },
                new User
                {
                    Id = 3,
                    Email = "customer123@yopmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
                    Role = "customer",
                    FirstName = "Customer",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow,
                    PhoneNumber = "9823016913"
                }
            );
        }
    }
}
