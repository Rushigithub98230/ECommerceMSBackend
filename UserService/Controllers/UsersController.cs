using EComMSSharedLibrary.ComonResponseModel;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Entity;

using UserService.Models;
using UserService.Services.UserServices;

namespace UserService.Controllers;

[ApiController]
[Route("/api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public UsersController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestModel model)
    {
        
        var response =  await _userService.CreateUser(model, model.Password);
        return response.ToActionResult();
       
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Models.LoginRequest model)
    {

        var response = await _userService.Authenticate(model.Email, model.Password);

        if (response.Success)
        {
            var token = GenerateJwtToken(response.Data);
            response.Data = null;
            response.AccessToken = token;
        }

        return response.ToActionResult();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
       
        var response = await _userService.GetById(id);
        
        return response.ToActionResult();
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _userService.GetAllUsers();
        return response.ToActionResult();
    }

    //[Authorize]
    //[HttpPut("{id}")]
    //public async Task<IActionResult> Update(int id, UpdateUserRequest model)
    //{
    //    // Only allow users to update their own data (unless admin)
    //    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    //    if (id != currentUserId && !User.IsInRole("admin"))
    //        return Forbid();

    //    var user = await _userService.GetById(id);
    //    if (user == null)
    //        return NotFound();

    //    // Update user properties
    //    user.FirstName = model.FirstName ?? user.FirstName;
    //    user.LastName = model.LastName ?? user.LastName;
    //    user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
    //    user.Email = model.Email ?? user.Email;

    //    try
    //    {
    //        await _userService.UpdateUser(user);
    //        return Ok(new { message = "User updated successfully" });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //}

    [Authorize]
    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(int id, ChangePasswordRequest model)
    {              
            var response = await _userService.ChangePassword(id, model.CurrentPassword, model.NewPassword);
            return response.ToActionResult();       
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
