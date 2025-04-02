using OrderService.Data.Repositories.IRepository;
using OrderService.Dtos;
using OrderService.Models;
using OrderService.Services.IService;
using System.Text.Json;

namespace OrderService.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IHttpClientFactory _httpClientFactory;
      //  private readonly IRabbitMQService _rabbitMQService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IHttpClientFactory httpClientFactory,
           // IRabbitMQService rabbitMQService,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _httpClientFactory = httpClientFactory;
          //  _rabbitMQService = rabbitMQService;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders.Select(o => MapToOrderDto(o));
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string customerId)
        {
            var orders = await _orderRepository.GetOrdersByCustomerAsync(customerId);
            return orders.Select(o => MapToOrderDto(o));
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            return order != null ? MapToOrderDto(order) : null;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto, string customerId)
        {
            // Validate order items and check stock
            if (orderDto.OrderItems == null || !orderDto.OrderItems.Any())
            {
                throw new ArgumentException("Order must contain at least one item");
            }

            var productClient = _httpClientFactory.CreateClient("ProductService");
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in orderDto.OrderItems)
            {
                // Check if product exists and is in stock
                var stockResponse = await productClient.GetAsync($"/api/products/{item.ProductId}/stock?quantity={item.Quantity}");
                if (!stockResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Failed to check stock for product {item.ProductId}");
                }

                var isInStock = await stockResponse.Content.ReadFromJsonAsync<bool>();
                if (!isInStock)
                {
                    throw new InvalidOperationException($"Product {item.ProductId} is not in stock with requested quantity");
                }

                // Get product details
                var productResponse = await productClient.GetAsync($"/api/products/{item.ProductId}");
                if (!productResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Failed to get product details for product {item.ProductId}");
                }

                var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();

                // Create order item
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity,
                    Subtotal = product.Price * item.Quantity
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.Subtotal;
            }

            // Create order
            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = OrderStatus.Processing,
                ShippingAddress = orderDto.ShippingAddress,
                OrderItems = orderItems
            };

            // Save order to database
            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            // Decrement stock for each product
            foreach (var item in orderDto.OrderItems)
            {
                var decrementStockResponse = await productClient.PostAsync(
                    $"/api/products/{item.ProductId}/decrementStock?quantity={item.Quantity}",
                    null);

                if (!decrementStockResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to decrement stock for product {item.ProductId}");
                    // In a real-world scenario, you might want to implement a compensation transaction here
                }
            }

            // Send notification via RabbitMQ
            await SendOrderNotification(createdOrder, "OrderCreated");

            return MapToOrderDto(createdOrder);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto statusDto)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                throw new ArgumentException($"Order with ID {id} not found");
            }

            // Update order status
            order.Status = statusDto.Status;

            if (!string.IsNullOrEmpty(statusDto.TrackingNumber))
            {
                order.TrackingNumber = statusDto.TrackingNumber;
            }

            // Update timestamps based on status
            if (statusDto.Status == OrderStatus.Shipped && !order.ShippedDate.HasValue)
            {
                order.ShippedDate = DateTime.UtcNow;
            }
            else if (statusDto.Status == OrderStatus.Delivered && !order.DeliveredDate.HasValue)
            {
                order.DeliveredDate = DateTime.UtcNow;
            }

            await _orderRepository.UpdateOrderAsync(order);

            // Send notification via RabbitMQ
            await SendOrderNotification(order, "OrderStatusUpdated");

            return MapToOrderDto(order);
        }

        public async Task<bool> CancelOrderAsync(int id, string customerId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                throw new ArgumentException($"Order with ID {id} not found");
            }

            // Verify customer owns this order
            if (order.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException("You are not authorized to cancel this order");
            }

            // Only allow cancellation if order is still processing
            if (order.Status != OrderStatus.Processing)
            {
                throw new InvalidOperationException("Cannot cancel order that has been shipped or delivered");
            }

            // Update order status
            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateOrderAsync(order);

            // Restore stock for each product
            var productClient = _httpClientFactory.CreateClient("ProductService");
            foreach (var item in order.OrderItems)
            {
                var restoreStockResponse = await productClient.PostAsync(
                    $"/api/products/{item.ProductId}/incrementStock?quantity={item.Quantity}",
                    null);

                if (!restoreStockResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to restore stock for product {item.ProductId}");
                }
            }

            // Send notification via RabbitMQ
            await SendOrderNotification(order, "OrderCancelled");

            return true;
        }

        private async Task SendOrderNotification(Order order, string eventType)
        {
            try
            {
                var notification = new
                {
                    EventType = eventType,
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status.ToString(),
                    Items = order.OrderItems.Select(item => new
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    }).ToList()
                };

                var message = JsonSerializer.Serialize(notification);
                //await _rabbitMQService.PublishMessageAsync("order_events", message);
                _logger.LogInformation($"Sent {eventType} notification for order {order.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send {eventType} notification for order {order.Id}");
            }
        }

        private OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                ShippingAddress = order.ShippingAddress,
                TrackingNumber = order.TrackingNumber,
                ShippedDate = order.ShippedDate,
                DeliveredDate = order.DeliveredDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    Subtotal = oi.Subtotal
                }).ToList()
            };
        }
    }
}
