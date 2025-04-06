namespace NotificationService.Dtos
{
    public class NotificationDto
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

    public class OrderCreatedMessage
    {
        public string Type { get; set; }
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemMessage> Items { get; set; }
    }

    public class OrderItemMessage
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public Guid SellerId { get; set; }
    }

    public class OrderStatusChangedMessage
    {
        public string Type { get; set; }
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string NewStatus { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
