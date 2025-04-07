using EComMSSharedLibrary.Models;
using NotificationServiceNew.Dtos;
using System.Net.Http.Headers;


namespace NotificationServiceNew.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _authToken;
        private DateTime _tokenExpiry = DateTime.MinValue;
        public UserService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ServiceUrls:UserService"]);
        }


        private async Task EnsureAuthenticated()
        {
            // Check if token is expired or not set
            if (string.IsNullOrEmpty(_authToken) || DateTime.UtcNow >= _tokenExpiry)
            {
                // Get a new token
                var response = await _httpClient.PostAsJsonAsync("api/Auth/service-token", new
                {
                    ServiceKey = _configuration["ServiceAuth:Key"]
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ServiceTokenResponse>();
                    _authToken = result.Token;
                    _tokenExpiry = DateTime.UtcNow.AddDays(29); // Set expiry a bit before the actual expiry

                    // Set the default authorization header for all requests
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                }
                else
                {
                    throw new Exception("Failed to authenticate with User Service");
                }
            }
        }


        public async Task<ApiResponse<UserDto>> GetUserAsync(Guid userId)
        {

            await EnsureAuthenticated();
            var response = await _httpClient.GetAsync($"api/Users/{userId}");
            if (!response.IsSuccessStatusCode)
                return ApiResponse<UserDto>.ErrorResponse("Failed to get user details", (int)response.StatusCode);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
            return apiResponse;
        }


        public class ServiceTokenResponse
        {
            public string Token { get; set; }
        }

    }
}
