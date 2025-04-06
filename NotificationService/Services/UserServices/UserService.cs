using EComMSSharedLibrary.Models;
using NotificationService.Dtos;

namespace NotificationService.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UserService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ServiceUrls:UserService"]);
        }

        public async Task<ApiResponse<UserDto>> GetUserAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"api/users/{userId}");
            if (!response.IsSuccessStatusCode)
                return ApiResponse<UserDto>.ErrorResponse("Failed to get user details", (int)response.StatusCode);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
            return apiResponse;
        }
    }
}
