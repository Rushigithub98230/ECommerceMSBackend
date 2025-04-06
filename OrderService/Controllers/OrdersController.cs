using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
using OrderService.Services;
using System.Security.Claims;

namespace OrderService.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _orderService.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("customer")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetByCustomerId()
    {
        var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var response = await _orderService.GetByCustomerIdAsync(customerId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("seller")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> GetBySellerId()
    {
        var sellerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var response = await _orderService.GetBySellerIdAsync(sellerId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto orderDto)
    {
        var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var response = await _orderService.CreateAsync(customerId, orderDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto updateStatusDto)
    {
        var response = await _orderService.UpdateStatusAsync(id, updateStatusDto);
        return StatusCode(response.StatusCode, response);
    }
}

