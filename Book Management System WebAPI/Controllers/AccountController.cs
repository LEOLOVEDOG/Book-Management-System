using Book_Management_System_WebAPI.Interfaces;
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
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AccountController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService; 
            _jwtService = jwtService;
        }

        [HttpPost("register")] // 註冊
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var result = await _userService.RegisterAsync(request.Username, request.Password, request.Email);
            if (!result.Success)
            {
                return BadRequest(new FailedResponse()
                {
                    Errors = result.Errors
                });
            }
            return Ok("Registration successful! Please check your email to verify your account.");
        }

        [HttpPost("login")] // 登入
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var result = await _userService.LoginAsync(request.Username, request.Password, request.RememberMe);
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
                RefreshToken = result.RefreshToken,
                TokenType = result.TokenType
            });
        }

        [HttpPost("logout")] // 登出
        public async Task<IActionResult> Logout([FromBody] InvalidateTokenRequest request)
        {
            var result = await _jwtService.InvalidateTokenAsync(request.RefreshToken);
            if (!result)
            {
                return Unauthorized("Invalid request.");
            }
            return Ok("User logged out successfully.");
        }

        [HttpPost("RefreshToken")] // 刷新 Token
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
                RefreshToken = result.RefreshToken,
                TokenType = result.TokenType
            });
        }

        [HttpGet("verify")] // 驗證郵件
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var result = await _jwtService.VerifyEmail(token);
            if (!result.Success)
            {
                return Unauthorized(new FailedResponse()
                {
                    Errors = result.Errors
                });
            }

            return Ok("Account successfully verified!");
        }

    }
}

