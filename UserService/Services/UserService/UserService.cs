using EComMSSharedLibrary.JwtTokenHandler;
using EComMSSharedLibrary.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.Data.DataAccessRepositories.IRepositories;
using UserService.DTOs;
using UserService.Models;

namespace UserService.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenHandler _jwtHelper;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtHelper = new JwtTokenHandler(configuration);
            _configuration = configuration;
        }

        public async Task<ApiResponse<UserDto>> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return ApiResponse<UserDto>.ErrorResponse("User not found", 404);

            return ApiResponse<UserDto>.SuccessResponse(MapToDto(user));
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(MapToDto);
            return ApiResponse<IEnumerable<UserDto>>.SuccessResponse(userDtos);
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterUserDto registerDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return ApiResponse<AuthResponseDto>.ErrorResponse("Email already in use", 400);

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = registerDto.Role
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var token = _jwtHelper.GenerateToken(createdUser.Id.ToString(), createdUser.Email, createdUser.Role);

            var response = new AuthResponseDto
            {
                Token = token,
                User = MapToDto(createdUser)
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "User registered successfully", 201);
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password", 401);

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password", 401);

            var token = _jwtHelper.GenerateToken(user.Id.ToString(), user.Email, user.Role);

            var response = new AuthResponseDto
            {
                Token = token,
                User = MapToDto(user)
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response);
        }

        public async Task<ApiResponse<UserDto>> UpdateAsync(Guid id, UserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return ApiResponse<UserDto>.ErrorResponse("User not found", 404);

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            

            var updatedUser = await _userRepository.UpdateAsync(user);
            return ApiResponse<UserDto>.SuccessResponse(MapToDto(updatedUser));
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            var result = await _userRepository.DeleteAsync(id);
            if (!result)
                return ApiResponse<bool>.ErrorResponse("User not found", 404);

            return ApiResponse<bool>.SuccessResponse(true, "User deleted successfully");
        }

        public async Task<string> GenerateServiceToken()
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, "service-account"),
        new Claim(ClaimTypes.Role, "Service")
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30), // Long-lived token for service
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}
