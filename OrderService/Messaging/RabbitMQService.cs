using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Text;

namespace OrderService.Messaging
{
    public class RabbitMQService : IRabbitMQService
    {
        //private readonly IConnection _connection;
        //private readonly IModel _channel;
        //private readonly ILogger<RabbitMQService> _logger;

        //public RabbitMQService(string hostName, string userName, string password, ILogger<RabbitMQService> logger = null)
        //{
        //    _logger = logger;
        //    try
        //    {
        //        var factory = new ConnectionFactory
        //        {
        //            HostName = hostName,
        //            UserName = userName,
        //            Password = password
        //        };

        //        _connection = factory.CreateConnection();
        //        _channel = _connection.CreateModel();

        //        // Declare exchanges and queues
        //        _channel.ExchangeDeclare("order_events", ExchangeType.Direct, durable: true);
        //        _channel.QueueDeclare("order_notifications", durable: true, exclusive: false, autoDelete: false);
        //        _channel.QueueBind("order_notifications", "order_events", "order_events");

        //        _logger?.LogInformation("RabbitMQ connection established");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger?.LogError(ex, "Failed to establish RabbitMQ connection");
        //        throw;
        //    }
        //}

        //public Task PublishMessageAsync(string queueName, string message)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            var body = Encoding.UTF8.GetBytes(message);
        //            _channel.BasicPublish(
        //                exchange: "order_events",
        //                routingKey: queueName,
        //                basicProperties: null,
        //                body: body);

        //            _logger?.LogInformation($"Message published to {queueName}");
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger?.LogError(ex, $"Failed to publish message to {queueName}");
        //            throw;
        //        }
        //    });
        //}

        //public void Dispose()
        //{
        //    _channel?.Close();
        //    _connection?.Close();
        //    _logger?.LogInformation("RabbitMQ connection closed");
        //}
    }
}
