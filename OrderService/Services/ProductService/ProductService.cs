using EComMSSharedLibrary.Models;
using OrderService.Dtos;
using System.Text;
using System.Text.Json;

namespace OrderService.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProductService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ServiceUrls:ProductService"]!);
        }

        public async Task<ApiResponse<ProductDto>> GetProductAsync(Guid productId)
        {
            var response = await _httpClient.GetAsync($"api/products/{productId}");
            if (!response.IsSuccessStatusCode)
                return ApiResponse<ProductDto>.ErrorResponse("Failed to get product details", (int)response.StatusCode);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> CheckStockAvailabilityAsync(Guid productId, int quantity)
        {
            var response = await _httpClient.GetAsync($"api/products/{productId}/stock/check?quantity={quantity}");
            if (!response.IsSuccessStatusCode)
                return ApiResponse<bool>.ErrorResponse("Failed to check stock availability", (int)response.StatusCode);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> DeductStockAsync(Guid productId, int quantity)
        {
            var product = await GetProductAsync(productId);
            if (!product.Success)
                return ApiResponse<bool>.ErrorResponse("Failed to get product details", product.StatusCode);

            var newQuantity = product.Data.StockQuantity - quantity;
            var updateStockDto = new { Quantity = newQuantity };

            var content = new StringContent(
                JsonSerializer.Serialize(updateStockDto),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync($"api/products/{productId}/stock", content);
            if (!response.IsSuccessStatusCode)
                return ApiResponse<bool>.ErrorResponse("Failed to update stock", (int)response.StatusCode);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            return apiResponse;
        }
    }
}
