using ProductService.DTos;
using ProductService.Models;
using ProductService.Repositories.IRepository;
using ProductService.Services.IService;

namespace ProductService.Services.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.Select(p => MapToProductDto(p));
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            return products.Select(p => MapToProductDto(p));
        }

        public async Task<IEnumerable<ProductDto>> GetProductsBySellerAsync(string sellerId)
        {
            var products = await _productRepository.GetProductsBySellerAsync(sellerId);
            return products.Select(p => MapToProductDto(p));
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            return product != null ? MapToProductDto(product) : null;
        }

        public async Task<ProductDto> AddProductAsync(CreateProductDto productDto, string sellerId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(productDto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException($"Category with ID {productDto.CategoryId} not found");
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                ImageUrl = productDto.ImageUrl,
                CategoryId = productDto.CategoryId,
                SellerId = sellerId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddProductAsync(product);
            _logger.LogInformation($"Product {product.Id} created by seller {sellerId}");

            return MapToProductDto(product);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto productDto, string sellerId)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                throw new ArgumentException($"Product with ID {id} not found");
            }

            if (existingProduct.SellerId != sellerId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this product");
            }

            if (productDto.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(productDto.CategoryId.Value);
                if (category == null)
                {
                    throw new ArgumentException($"Category with ID {productDto.CategoryId} not found");
                }
                existingProduct.CategoryId = productDto.CategoryId.Value;
            }

            // Update properties if provided
            if (!string.IsNullOrEmpty(productDto.Name))
                existingProduct.Name = productDto.Name;

            if (!string.IsNullOrEmpty(productDto.Description))
                existingProduct.Description = productDto.Description;

            if (productDto.Price.HasValue)
                existingProduct.Price = productDto.Price.Value;

            if (productDto.StockQuantity.HasValue)
                existingProduct.StockQuantity = productDto.StockQuantity.Value;

            if (!string.IsNullOrEmpty(productDto.ImageUrl))
                existingProduct.ImageUrl = productDto.ImageUrl;

            if (productDto.IsActive.HasValue)
                existingProduct.IsActive = productDto.IsActive.Value;

            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateProductAsync(existingProduct);
            _logger.LogInformation($"Product {id} updated by seller {sellerId}");

            return MapToProductDto(existingProduct);
        }

        public async Task DeleteProductAsync(int id, string? sellerId)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                throw new ArgumentException($"Product with ID {id} not found");
            }

            if (existingProduct.SellerId != sellerId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this product");
            }

            await _productRepository.DeleteProductAsync(id);
            _logger.LogInformation($"Product {id} deleted by seller {sellerId}");
        }

        public async Task<bool> IsInStockAsync(int id, int quantity)
        {
            return await _productRepository.IsInStockAsync(id, quantity);
        }

        public async Task<bool> DecrementStockAsync(int id, int quantity)
        {
            return await _productRepository.DecrementStockAsync(id, quantity);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm);
            return products.Select(p => MapToProductDto(p));
        }

        private ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                SellerId = product.SellerId,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
