using NotificationService.Models;

namespace NotificationService.Services.EmailServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(Notification notification);
    }
}
