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
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] string search = null)
    {
        if (!string.IsNullOrEmpty(search))
        {
            var searchResults = await _productService.SearchProductsAsync(search);
            return Ok(searchResults);
        }

        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpGet("seller")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetSellerProducts()
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var products = await _productService.GetProductsBySellerAsync(sellerId);
        return Ok(products);
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto productDto)
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _productService.AddProductAsync(productDto, sellerId);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto productDto)
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _productService.UpdateProductAsync(id, productDto, sellerId);
            return Ok(product);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _productService.DeleteProductAsync(id, sellerId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("{id}/stock")]
    public async Task<ActionResult<bool>> CheckStock(int id, [FromQuery] int quantity = 1)
    {
        var isInStock = await _productService.IsInStockAsync(id, quantity);
        return Ok(isInStock);
    }
}
