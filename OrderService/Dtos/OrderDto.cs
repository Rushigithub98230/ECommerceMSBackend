using OrderService.Models;

namespace OrderService.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }


    public class CreateOrderDto
    {
        public string ShippingAddress { get; set; }
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }

    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
        public string TrackingNumber { get; set; }
    }
}
