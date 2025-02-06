using Book_Management_System.Services;
using Book_Management_System_WebAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Management_System_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public AccountController(UserService userService, JwtService jwtService)
        {
            _userService = userService; 
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 檢查是否能成功註冊
            var userRegistered = await _userService.RegisterUserAsync(registerDTO.Username, registerDTO.Password, registerDTO.Email);
            if (!userRegistered)
            {
                return BadRequest(new { Message = "Username already exists. Please try another one." });
            }

            return Ok(new
            {
                Message = "User registered successfully. Please login to access your account.",
                Username = registerDTO.Username
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.ValidateUserAsync(loginDTO.Username, loginDTO.Password ,loginDTO.RememberMe);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var roles = user.Roles.Select(r => r.Name).ToList();
            var token = _jwtService.GenerateToken(user.Username, roles);

            return Ok(new { Message = "Login successful.", Token = token });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok("User logged out successfully.");
        }
    }
}

