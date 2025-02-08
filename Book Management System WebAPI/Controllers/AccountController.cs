using Book_Management_System.Services;
using Book_Management_System_WebAPI.Requests;
using Book_Management_System_WebAPI.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Management_System_WebAPI.Controllers
{
    [AllowAnonymous]
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 檢查是否能成功註冊
            var user = await _userService.RegisterAsync(request.Username, request.Password, request.Email);
            if (!user)
            {
                return BadRequest(new { Message = "Username already exists. Please try another one." });
            }

            return Ok(new
            {
                Message = "User registered successfully. Please login to access your account.",
                Username = request.Username
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.LoginAsync(request.Username, request.Password ,request.RememberMe);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var roles = user.Roles.Select(r => r.Name).ToList();
            var token = _jwtService.GenerateToken(user.Username, roles);

            return Ok(new { Message = "Login successful.", token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] InvalidateTokenRequest request)
        {
            var user = await _jwtService.InvalidateTokenAsync(request.RefreshToken);
            if (!user)
            {
                return Unauthorized("Invalid request.");
            }
            return Ok("User logged out successfully.");
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var result = await _jwtService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);
            if (!result.Success)
            {
                return Unauthorized(new FailedResponse()
                {
                    Errors = result.Errors
                });
            }

            return Ok(new TokenResponse
            {
                AccessToken = result.AccessToken,
                TokenType = result.TokenType,
                ExpiresIn = result.ExpireMinutes,
                RefreshToken = result.RefreshToken
            });
        }

    }
}

