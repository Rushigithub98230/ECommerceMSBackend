using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTos;
using ProductService.Services.IService;
using System.Security.Claims;

namespace ProductService.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var response = await _productService.GetAllAsync();
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _productService.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("seller/{sellerId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySellerId(Guid sellerId)
    {
        var response = await _productService.GetBySellerIdAsync(sellerId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCategoryId(Guid categoryId)
    {
        var response = await _productService.GetByCategoryIdAsync(categoryId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(Roles = "seller,admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto productDto)
    {
        var sellerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var response = await _productService.CreateAsync(sellerId, productDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "seller,admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto productDto)
    {
        var sellerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var response = await _productService.UpdateAsync(id, sellerId, productDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "seller,admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var sellerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var response = await _productService.DeleteAsync(id, sellerId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id}/stock")]
    [Authorize(Roles = "seller,admin")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockDto updateStockDto)
    {
        var response = await _productService.UpdateStockAsync(id, updateStockDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}/stock/check")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckStockAvailability(Guid id, [FromQuery] int quantity)
    {
        var response = await _productService.CheckStockAvailabilityAsync(id, quantity);
        return StatusCode(response.StatusCode, response);
    }
}
