using EComMSSharedLibrary.Models;
using NotificationServiceNew.Dtos;


namespace NotificationServiceNew.Services.UserServices
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetUserAsync(Guid userId);
    }
}
