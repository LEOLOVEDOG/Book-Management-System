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

        [HttpPost("Register")] // 註冊
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

        [HttpPost("Login")] // 登入
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
                TokenType = result.TokenType,
                ExpiresIn = result.ExpireMinutes
            });
        }

        [HttpPost("Logout")] // 登出
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
                TokenType = result.TokenType,
                ExpiresIn = result.ExpireMinutes
            });
        }

        [HttpGet("Verify")] // 驗證郵件
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

        [HttpPost("ResendEmail")] // 重新發送驗證郵件
        public async Task<IActionResult> ResendVerificationEmail([FromBody] string email)
        {
            var result = await _userService.SendVerificationEmailAsync(email, "verify");
            if (!result.Success)
            {
                return BadRequest(new FailedResponse()
                {
                    Errors = result.Errors
                });
            }

            return Ok("Verification email sent successfully.");
        }

        [HttpPost("ForgotPassword")] // 忘記密碼
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var result = await _userService.SendVerificationEmailAsync(email, "resetPassword");
            if (!result.Success)
            {
                return BadRequest(new FailedResponse()
                {
                    Errors = result.Errors
                });
            }

            return Ok("Password reset email sent successfully.");
        }

        [HttpPost("ResetPassword")] // 重設密碼
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var resetPasswordRequest = new ResetPasswordRequest
            {
                Email = request.Email,
                NewPassword = request.NewPassword,
                Token = request.Token
            };

            var result = await _userService.ResetPasswordAsync(resetPasswordRequest);
            if (!result.Success)
            {
                return BadRequest(new FailedResponse()
                {
                    Errors = result.Errors
                });
            }

            return Ok("Password reset successfully.");
        }
    }
}

