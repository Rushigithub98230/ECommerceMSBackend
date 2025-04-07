
using EComMSSharedLibrary.JwtTokenHandler;
using EComMSSharedLibrary.Models;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using UserService.DTOs;


using UserService.Models;
using UserService.Services.UserService;


namespace UserService.Controllers;

[ApiController]
[Route("/api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly JwtTokenHandler _jwtHelper;
    public UsersController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
        _jwtHelper = new JwtTokenHandler(configuration);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll()
    {
        var response = await _userService.GetAllAsync();
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _userService.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        var response = await _userService.RegisterAsync(registerDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _userService.LoginAsync(loginDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("service-token")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServiceToken([FromBody] ServiceTokenRequest request)
    {
        // Validate the service key
        if (request.ServiceKey != _configuration["ServiceAuth:Key"])
        {
            return Unauthorized(new { message = "Invalid service key" });
        }

        var token = await _jwtHelper.GenerateServiceToken();
        return Ok(new { token });
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserDto userDto)
    {
        var response = await _userService.UpdateAsync(id, userDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _userService.DeleteAsync(id);
        return StatusCode(response.StatusCode, response);
    }
}
