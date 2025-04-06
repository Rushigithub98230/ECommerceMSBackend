using EComMSSharedLibrary.Models;
using ProductService.Data.DataAccessRepositories.categoryRepositories;
using ProductService.Data.DataAccessRepositories.ProductRepositories;
using ProductService.DTos;
using ProductService.Models;

using ProductService.Services.IService;

namespace ProductService.Services.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponse<ProductDto>> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || !product.IsActive)
                return ApiResponse<ProductDto>.ErrorResponse("Product not found", 404);

            return ApiResponse<ProductDto>.SuccessResponse(MapToDto(product));
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productDtos = products.Select(MapToDto);
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos);
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetBySellerIdAsync(Guid sellerId)
        {
            var products = await _productRepository.GetBySellerIdAsync(sellerId);
            var productDtos = products.Select(MapToDto);
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos);
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetByCategoryIdAsync(Guid categoryId)
        {
            var categoryExists = await _categoryRepository.ExistsAsync(categoryId);
            if (!categoryExists)
                return ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("Category not found", 404);

            var products = await _productRepository.GetByCategoryIdAsync(categoryId);
            var productDtos = products.Select(MapToDto);
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos);
        }

        public async Task<ApiResponse<ProductDto>> CreateAsync(Guid sellerId, CreateProductDto productDto)
        {
            // Verify category exists
            var categoryExists = await _categoryRepository.ExistsAsync(productDto.CategoryId);
            if (!categoryExists)
                return ApiResponse<ProductDto>.ErrorResponse("Category not found", 404);

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                SellerId = sellerId,
                CategoryId = productDto.CategoryId,
                ImageUrls = productDto.ImageUrls
            };

            var createdProduct = await _productRepository.CreateAsync(product);

            // Fetch the complete product with category
            var productWithCategory = await _productRepository.GetByIdAsync(createdProduct.Id);

            return ApiResponse<ProductDto>.SuccessResponse(MapToDto(productWithCategory), "Product created successfully", 201);
        }

        public async Task<ApiResponse<ProductDto>> UpdateAsync(Guid id, Guid sellerId, UpdateProductDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || !product.IsActive)
                return ApiResponse<ProductDto>.ErrorResponse("Product not found", 404);

            if (product.SellerId != sellerId)
                return ApiResponse<ProductDto>.ErrorResponse("You are not authorized to update this product", 403);

            // Verify category exists
            var categoryExists = await _categoryRepository.ExistsAsync(productDto.CategoryId);
            if (!categoryExists)
                return ApiResponse<ProductDto>.ErrorResponse("Category not found", 404);

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.StockQuantity = productDto.StockQuantity;
            product.CategoryId = productDto.CategoryId;
            product.ImageUrls = productDto.ImageUrls;

            var updatedProduct = await _productRepository.UpdateAsync(product);

            
            var productWithCategory = await _productRepository.GetByIdAsync(updatedProduct.Id);

            return ApiResponse<ProductDto>.SuccessResponse(MapToDto(productWithCategory));
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id, Guid sellerId)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || !product.IsActive)
                return ApiResponse<bool>.ErrorResponse("Product not found", 404);

            if (product.SellerId != sellerId)
                return ApiResponse<bool>.ErrorResponse("You are not authorized to delete this product", 403);

            var result = await _productRepository.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(result, "Product deleted successfully");
        }

        public async Task<ApiResponse<bool>> UpdateStockAsync(Guid id, UpdateStockDto updateStockDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || !product.IsActive)
                return ApiResponse<bool>.ErrorResponse("Product not found", 404);

            var result = await _productRepository.UpdateStockAsync(id, updateStockDto.Quantity);
            return ApiResponse<bool>.SuccessResponse(result, "Stock updated successfully");
        }

        public async Task<ApiResponse<bool>> CheckStockAvailabilityAsync(Guid id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || !product.IsActive)
                return ApiResponse<bool>.ErrorResponse("Product not found", 404);

            var isAvailable = product.StockQuantity >= quantity;
            return ApiResponse<bool>.SuccessResponse(isAvailable);
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                SellerId = product.SellerId,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                ImageUrls = product.ImageUrls
            };
        }
    }
}
