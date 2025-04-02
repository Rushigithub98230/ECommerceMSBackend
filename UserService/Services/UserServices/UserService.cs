using EComMSSharedLibrary.ComonResponseModel;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using UserService.Data;
using UserService.Entity;
using UserService.Models;

namespace UserService.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;

        public UserService(UserDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<User>> Authenticate(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return ApiResponse<User>.Create(null, "Email and password must be provided.",
                    StatusCodes.Status400BadRequest);
            }

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return ApiResponse<User>.Create(null, "User not found.",
                    StatusCodes.Status404NotFound);
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return ApiResponse<User>.Create(null, "Incorrect password.",
                    StatusCodes.Status401Unauthorized);
            }

            return ApiResponse<User>.Create(user, "Authentication successful.", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<User>> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            return user != null
                ? ApiResponse<User>.Create(user, "User retrieved successfully.", StatusCodes.Status200OK)
                : ApiResponse<User>.Create(null, "User not found.", StatusCodes.Status404NotFound
                   );
        }

        public async Task<ApiResponse<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            return users.Any()
                ? ApiResponse<IEnumerable<User>>.Create(users, "Users retrieved successfully.", StatusCodes.Status200OK)
                : ApiResponse<IEnumerable<User>>.Create(null,"No users found.", StatusCodes.Status404NotFound,
                    new List<string> { "There are no users in the database." });
        }

        public async Task<ApiResponse<User>> CreateUser(RegisterRequestModel model, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return ApiResponse<User>.Create(null,"Password is required", StatusCodes.Status400BadRequest);
            }

            string validationMessage = ValidatePassword(password);
            if (validationMessage != "Valid")
            {
                return ApiResponse<User>.Create(null, validationMessage, StatusCodes.Status400BadRequest);
            }

            if (await UserExists(model.Email) || await _context.Users.AnyAsync(x => x.Email == model.Email))
            {
                return ApiResponse<User>.Create(null, $"Email '{model.Email}' is already taken", StatusCodes.Status409Conflict);
            }

            var user = new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Role = string.IsNullOrEmpty(model.Role) ? "customer" : model.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return ApiResponse<User>.Create(user, "User created successfully", StatusCodes.Status201Created);
        }

        public async Task<bool> UserExists(string email)
        {
            var exist = await _context.Users.AnyAsync(x => x.Email == email);
            return exist;

        }


        public async Task<ApiResponse<User>> UpdateUser(User userParam)
        {
            var user = await _context.Users.FindAsync(userParam.Id);
            if (user == null)
            {
                return ApiResponse<User>.Create(null,"User not found.", StatusCodes.Status404NotFound);
            }

            
            if (!string.IsNullOrWhiteSpace(userParam.Email) && userParam.Email != user.Email)
            {
                if (await _context.Users.AnyAsync(x => x.Email == userParam.Email))
                {
                    return ApiResponse<User>.Create(null,$"Email '{userParam.Email}' is already taken.", StatusCodes.Status400BadRequest);
                }
                user.Email = userParam.Email;
            }

           
            user.FirstName = !string.IsNullOrWhiteSpace(userParam.FirstName) ? userParam.FirstName : user.FirstName;
            user.LastName = !string.IsNullOrWhiteSpace(userParam.LastName) ? userParam.LastName : user.LastName;
            user.PhoneNumber = !string.IsNullOrWhiteSpace(userParam.PhoneNumber) ? userParam.PhoneNumber : user.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return ApiResponse<User>.Create(user, "User updated successfully.", StatusCodes.Status200OK);
        }

        
        public async Task<ApiResponse<Object>> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return ApiResponse<Object>.Create(null, "User not found.", StatusCodes.Status404NotFound);
            }

            
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return ApiResponse<Object>.Create(false, "Current password is incorrect.", StatusCodes.Status401Unauthorized);
            }

            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return ApiResponse<Object>.Create(null, "Password changed successfully.", StatusCodes.Status200OK);
        }


        private string ValidatePassword(string password)
        {
            if (password.Length < 8)
                return "Password must be at least 8 characters long.";

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return "Password must contain at least one uppercase letter.";

            if (!Regex.IsMatch(password, @"[a-z]"))
                return "Password must contain at least one lowercase letter.";

            if (!Regex.IsMatch(password, @"\d"))
                return "Password must contain at least one number.";

            if (!Regex.IsMatch(password, @"[@$!%*?&#]"))
                return "Password must contain at least one special character (@$!%*?&#).";

            return "Valid";
        }
    }
}
