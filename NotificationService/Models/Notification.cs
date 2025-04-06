namespace NotificationService.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Type { get; set; } 
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
