using NotificationService.Models;
using NotificationService.Repositories;
using System.Net.Mail;
using System.Net;

namespace NotificationService.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration,
            INotificationRepository notificationRepository,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(Notification notification)
        {
            try
            {
                var smtpServer = _configuration["Email:SmtpServer"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
                var smtpUsername = _configuration["Email:Username"];
                var smtpPassword = _configuration["Email:Password"];
                var senderEmail = _configuration["Email:SenderEmail"];
                var senderName = _configuration["Email:SenderName"];

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = notification.Subject,
                    Body = notification.Content,
                    IsBodyHtml = false
                };

                message.To.Add(notification.Recipient);

                await client.SendMailAsync(message);
                await _notificationRepository.UpdateSentStatusAsync(notification.Id, true);

                _logger.LogInformation($"Email sent to {notification.Recipient} with subject: {notification.Subject}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {notification.Recipient}");
                return false;
            }
        }
    }
}
