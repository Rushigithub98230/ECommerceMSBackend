using NotificationService.Dtos;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using NotificationService.Services;
using System.Text;

namespace NotificationService.Messaging
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQConsumer(
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILogger<RabbitMQConsumer> logger)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"],
                Port = int.Parse(_configuration["RabbitMQ:Port"]),
                UserName = _configuration["RabbitMQ:Username"],
                Password = _configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "order_notifications",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation("RabbitMQ connection initialized");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, eventArgs) =>
            {
                try
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation($"Received message: {message}");

                    await ProcessMessage(message);

                    _channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queue: "order_notifications",
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessMessage(string message)
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var messageType = JsonDocument.Parse(message).RootElement.GetProperty("Type").GetString();

            switch (messageType)
            {
                case "OrderCreated":
                    var orderCreatedMessage = JsonSerializer.Deserialize<OrderCreatedMessage>(message);
                    await notificationService.ProcessOrderCreatedMessageAsync(orderCreatedMessage);
                    break;

                case "OrderStatusChanged":
                    var orderStatusChangedMessage = JsonSerializer.Deserialize<OrderStatusChangedMessage>(message);
                    await notificationService.ProcessOrderStatusChangedMessageAsync(orderStatusChangedMessage);
                    break;

                default:
                    _logger.LogWarning($"Unknown message type: {messageType}");
                    break;
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
