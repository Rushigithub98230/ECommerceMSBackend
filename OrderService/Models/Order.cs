using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    
        public class Order
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public string CustomerId { get; set; }

            [Required]
            public DateTime OrderDate { get; set; } = DateTime.UtcNow;

            [Required]
            [Column(TypeName = "decimal(18,2)")]
            public decimal TotalAmount { get; set; }

            [Required]
            public OrderStatus Status { get; set; } = OrderStatus.Processing;

            [StringLength(500)]
            public string ShippingAddress { get; set; }

            [StringLength(20)]
            public string TrackingNumber { get; set; }

            public DateTime? ShippedDate { get; set; }

            public DateTime? DeliveredDate { get; set; }

            public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        }

        public enum OrderStatus
        {
            Processing = 0,
            Shipped = 1,
            Delivered = 2,
            Cancelled = 3
        }
    
}
