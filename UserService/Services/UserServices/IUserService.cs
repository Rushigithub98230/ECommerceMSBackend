using EComMSSharedLibrary.ComonResponseModel;
using UserService.Entity;
using UserService.Models;

namespace UserService.Services.UserServices
{
    public interface IUserService
    {
        Task<ApiResponse<User>> Authenticate(string email, string password);
        Task<ApiResponse<User>> GetById(int id);
        Task<ApiResponse<IEnumerable<User>>> GetAllUsers();
        Task<ApiResponse<User>> CreateUser(RegisterRequestModel user, string password);
        Task<bool> UserExists(string email);
        Task<ApiResponse<User>> UpdateUser(User userParam);
        Task<ApiResponse<Object>> ChangePassword(int userId, string currentPassword, string newPassword);
    }
}
