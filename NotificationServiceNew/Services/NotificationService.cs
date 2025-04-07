using NotificationServiceNew.Dtos;
using NotificationServiceNew.Models;
using NotificationServiceNew.Repositories;
using NotificationServiceNew.Services.EmailServices;
using NotificationServiceNew.Services.UserServices;

namespace NotificationServiceNew.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailService _emailService;
       
        private readonly IUserService _userService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IEmailService emailService,
            
            IUserService userService,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _emailService = emailService;
            
            _userService = userService;
            _logger = logger;
        }

        public async Task ProcessOrderCreatedMessageAsync(OrderCreatedMessage message)
        {
            try
            {
                // Get customer details
                var customerResponse = await _userService.GetUserAsync(message.CustomerId);
                if (!customerResponse.Success)
                {
                    _logger.LogError($"Failed to get customer details for order {message.OrderId}");
                    return;
                }

                var customer = customerResponse.Data;

                // Create email notification for customer
                var customerEmailContent = $"Dear {customer.FirstName},\n\n" +
                    $"Thank you for your order (#{message.OrderId}).\n" +
                    $"Your order has been received and is now being processed.\n\n" +
                    $"Order Details:\n" +
                    $"Total Amount: ${message.TotalAmount}\n" +
                    $"Status: {message.Status}\n\n" +
                    $"Thank you for shopping with us!";

                var customerEmailNotification = new Notification
                {
                    Type = "Email",
                    Recipient = customer.Email,
                    Subject = $"Order Confirmation - #{message.OrderId}",
                    Content = customerEmailContent
                };

                await _notificationRepository.CreateAsync(customerEmailNotification);
                await _emailService.SendEmailAsync(customerEmailNotification);

                // Create notifications for sellers
                var sellerIds = message.Items.Select(i => i.SellerId).Distinct();
                foreach (var sellerId in sellerIds)
                {
                    var sellerResponse = await _userService.GetUserAsync(sellerId);
                    if (!sellerResponse.Success)
                    {
                        _logger.LogError($"Failed to get seller details for seller {sellerId}");
                        continue;
                    }

                    var seller = sellerResponse.Data;
                    var sellerItems = message.Items.Where(i => i.SellerId == sellerId).ToList();

                    var sellerEmailContent = $"Dear {seller.FirstName},\n\n" +
                        $"You have received a new order (#{message.OrderId}).\n\n" +
                        $"Order Items:\n";

                    foreach (var item in sellerItems)
                    {
                        sellerEmailContent += $"- {item.ProductName} (Quantity: {item.Quantity})\n";
                    }

                    sellerEmailContent += $"\nPlease process this order as soon as possible.";

                    var sellerEmailNotification = new Notification
                    {
                        Type = "Email",
                        Recipient = seller.Email,
                        Subject = $"New Order Received - #{message.OrderId}",
                        Content = sellerEmailContent
                    };

                    await _notificationRepository.CreateAsync(sellerEmailNotification);
                    await _emailService.SendEmailAsync(sellerEmailNotification);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing order created message for order {message.OrderId}");
            }
        }

        public async Task ProcessOrderStatusChangedMessageAsync(OrderStatusChangedMessage message)
        {
            try
            {
               
                var customerResponse = await _userService.GetUserAsync(message.CustomerId);
                if (!customerResponse.Success)
                {
                    _logger.LogError($"Failed to get customer details for order {message.OrderId}");
                    return;
                }

                var customer = customerResponse.Data;

                
                var customerEmailContent = $"Dear {customer.FirstName},\n\n" +
                    $"Your order (#{message.OrderId}) status has been updated to {message.NewStatus}.\n\n" +
                    $"If you have any questions, please contact our customer support.\n\n" +
                    $"Thank you for shopping with us!";

                var customerEmailNotification = new Notification
                {
                    Type = "Email",
                    Recipient = customer.Email,
                    Subject = $"Order Status Update - #{message.OrderId}",
                    Content = customerEmailContent
                };

                await _notificationRepository.CreateAsync(customerEmailNotification);
                await _emailService.SendEmailAsync(customerEmailNotification);

                
                if (message.NewStatus == "Shipped")
                {
                    var smsContent = $"Your order #{message.OrderId} has been shipped and is on its way to you!";

                    var smsNotification = new Notification
                    {
                        Type = "SMS",
                        Recipient = customer.PhoneNumber,
                        Subject = "Order Shipped",
                        Content = smsContent
                    };

                    await _notificationRepository.CreateAsync(smsNotification);
                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing order status changed message for order {message.OrderId}");
            }
        }
    }
}
