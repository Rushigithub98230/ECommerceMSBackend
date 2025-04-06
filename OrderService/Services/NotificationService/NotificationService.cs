using OrderService.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task SendOrderCreatedNotificationAsync(Order order)
        {
            var message = new
            {
                Type = "OrderCreated",
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    SellerId = i.SellerId
                }).ToList()
            };

            PublishMessage("order_notifications", message);
            return Task.CompletedTask;
        }

        public Task SendOrderStatusChangedNotificationAsync(Order order)
        {
            var message = new
            {
                Type = "OrderStatusChanged",
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                NewStatus = order.Status,
                UpdatedAt = order.UpdatedAt
            };

            PublishMessage("order_notifications", message);
            return Task.CompletedTask;
        }

        private void PublishMessage(string queueName, object message)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:Host"],
                    Port = int.Parse(_configuration["RabbitMQ:Port"]),
                    UserName = _configuration["RabbitMQ:Username"],
                    Password = _configuration["RabbitMQ:Password"]
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var jsonMessage = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: null,
                    body: body);

                _logger.LogInformation($"Message sent to queue {queueName}: {jsonMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to publish message to queue {queueName}");
            }
        }
    }
}
