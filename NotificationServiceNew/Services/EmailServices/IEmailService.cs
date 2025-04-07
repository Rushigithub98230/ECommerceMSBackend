using NotificationServiceNew.Models;

namespace NotificationServiceNew.Services.EmailServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(Notification notification);
    }
}
