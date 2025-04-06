using EComMSSharedLibrary.Models;
using NotificationService.Dtos;

namespace NotificationService.Services.UserServices
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetUserAsync(Guid userId);
    }
}
