using EComMSSharedLibrary.Models;
using OrderService.Data.DataAccessRepositories;
using OrderService.Dtos;
using OrderService.Models;
using OrderService.Services.NotificationService;
using OrderService.Services.ProductService;

namespace OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;

        public OrderService(
            IOrderRepository orderRepository,
            IProductService productService,
            INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<OrderDto>> GetByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return ApiResponse<OrderDto>.ErrorResponse("Order not found", 404);

            return ApiResponse<OrderDto>.SuccessResponse(MapToDto(order));
        }

        public async Task<ApiResponse<IEnumerable<OrderDto>>> GetByCustomerIdAsync(Guid customerId)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
            var orderDtos = orders.Select(MapToDto);
            return ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orderDtos);
        }

        public async Task<ApiResponse<IEnumerable<OrderDto>>> GetBySellerIdAsync(Guid sellerId)
        {
            var orders = await _orderRepository.GetBySellerIdAsync(sellerId);
            var orderDtos = orders.Select(MapToDto);
            return ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orderDtos);
        }

        public async Task<ApiResponse<OrderDto>> CreateAsync(Guid customerId, CreateOrderDto orderDto)
        {
           
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in orderDto.Items)
            {
               
                var stockResponse = await _productService.CheckStockAvailabilityAsync(item.ProductId, item.Quantity);
                if (!stockResponse.Success || !stockResponse.Data)
                    return ApiResponse<OrderDto>.ErrorResponse($"Product {item.ProductId} is out of stock or has insufficient quantity", 400);

               
                var productResponse = await _productService.GetProductAsync(item.ProductId);
                if (!productResponse.Success)
                    return ApiResponse<OrderDto>.ErrorResponse($"Failed to get product details for {item.ProductId}", 400);

                var product = productResponse.Data;

               
                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    SellerId = product.SellerId
                };

                orderItems.Add(orderItem);
                totalAmount += product.Price * item.Quantity;
            }

           
            var order = new Order
            {
                CustomerId = customerId,
                TotalAmount = totalAmount,
                ShippingAddress = orderDto.ShippingAddress,
                
                Items = orderItems
            };

            var createdOrder = await _orderRepository.CreateAsync(order);

            
            foreach (var item in orderItems)
            {
                await _productService.DeductStockAsync(item.ProductId, item.Quantity);
            }

          
            await _notificationService.SendOrderCreatedNotificationAsync(createdOrder);

            return ApiResponse<OrderDto>.SuccessResponse(MapToDto(createdOrder), "Order created successfully", 201);
        }

        public async Task<ApiResponse<OrderDto>> UpdateStatusAsync(Guid id, UpdateOrderStatusDto updateStatusDto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return ApiResponse<OrderDto>.ErrorResponse("Order not found", 404);

            var updatedOrder = await _orderRepository.UpdateStatusAsync(id, updateStatusDto.Status);

            
            await _notificationService.SendOrderStatusChangedNotificationAsync(updatedOrder);

            return ApiResponse<OrderDto>.SuccessResponse(MapToDto(updatedOrder), "Order status updated successfully");
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                ShippingAddress = order.ShippingAddress,             
                Items = order.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    SellerId = item.SellerId
                }).ToList()
            };
        }
    }
}
