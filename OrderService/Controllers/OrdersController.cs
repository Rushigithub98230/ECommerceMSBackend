using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
using OrderService.Services.IService;
using System.Security.Claims;

namespace OrderService.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderService orderService,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin,Seller")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [Authorize]
    [HttpGet("customer")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetCustomerOrders()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
        return Ok(orders);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        // If not admin or seller, check if the order belongs to the current user
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (userRole != "Admin" && userRole != "Seller" && order.CustomerId != customerId)
        {
            return Forbid();
        }

        return Ok(order);
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto orderDto)
    {
        try
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _orderService.CreateOrderAsync(orderDto, customerId);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto statusDto)
    {
        try
        {
            var order = await _orderService.UpdateOrderStatusAsync(id, statusDto);
            return Ok(order);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        try
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.CancelOrderAsync(id, customerId);
            return Ok(new { success = result });
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
