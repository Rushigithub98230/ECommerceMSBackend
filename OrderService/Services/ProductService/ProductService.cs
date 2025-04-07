using EComMSSharedLibrary.Models;
using OrderService.Dtos;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OrderService.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClientProduct;
        private readonly HttpClient _httpClientUser;
        private readonly IConfiguration _configuration;
        private string _authToken;
        private DateTime _tokenExpiry = DateTime.MinValue;

        public ProductService(HttpClient httpClientProduct, IConfiguration configuration, HttpClient httpClientUser)
        {
            _httpClientProduct = httpClientProduct;
            _httpClientUser = httpClientUser;
            _configuration = configuration;
            _httpClientProduct.BaseAddress = new Uri(_configuration["ServiceUrls:ProductService"]!);
            _httpClientUser.BaseAddress = new Uri(_configuration["ServiceUrls:UserService"]!);

        }


        public async Task<ApiResponse<ProductDto>> GetProductAsync(Guid productId)
        {

            var response = await _httpClientProduct.GetAsync($"api/products/{productId}");
            if (!response.IsSuccessStatusCode)
                return ApiResponse<ProductDto>.ErrorResponse("Failed to get product details", (int)response.StatusCode);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> CheckStockAvailabilityAsync(Guid productId, int quantity)
        {
            await EnsureAuthenticated();

            var response = await _httpClientProduct.GetAsync($"api/products/{productId}/stock/check?quantity={quantity}");
            if (!response.IsSuccessStatusCode)
                return ApiResponse<bool>.ErrorResponse("Failed to check stock availability", (int)response.StatusCode);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> DeductStockAsync(Guid productId, int quantity)
        {
            await EnsureAuthenticated();

            var product = await GetProductAsync(productId);
            if (!product.Success)
                return ApiResponse<bool>.ErrorResponse("Failed to get product details", product.StatusCode);

            var newQuantity = product.Data.StockQuantity - quantity;
            var updateStockDto = new { Quantity = newQuantity };

            var content = new StringContent(
                JsonSerializer.Serialize(updateStockDto),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClientProduct.PutAsync($"api/products/{productId}/stock", content);
            if (!response.IsSuccessStatusCode)
                return ApiResponse<bool>.ErrorResponse("Failed to update stock", (int)response.StatusCode);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            return apiResponse;
        }

        private async Task EnsureAuthenticated()
        {

            if (string.IsNullOrEmpty(_authToken) || DateTime.UtcNow >= _tokenExpiry)
            {

                var response = await _httpClientUser.PostAsJsonAsync("api/Users/service-token", new
                {
                    ServiceKey = _configuration["ServiceAuth:Key"]
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ServiceTokenResponse>();
                    _authToken = result.Token;
                    _tokenExpiry = DateTime.UtcNow.AddDays(29);


                    _httpClientProduct.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                }
                else
                {
                    throw new Exception("Failed to authenticate with User Service");
                }
            }

        }

        public class ServiceTokenResponse
        {
            public string Token { get; set; }
        }
    }
}
