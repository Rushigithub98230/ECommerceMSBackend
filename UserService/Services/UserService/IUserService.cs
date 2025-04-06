using EComMSSharedLibrary.Models;
using UserService.DTOs;

namespace UserService.Services.UserService
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync();
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterUserDto registerDto);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<UserDto>> UpdateAsync(Guid id, UserDto userDto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
