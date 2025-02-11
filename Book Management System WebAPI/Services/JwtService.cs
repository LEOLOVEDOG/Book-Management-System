using Book_Management_System_WebAPI.Models;
using Book_Management_System_WebAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Book_Management_System.Services
{
    public class JwtService
    {
        private readonly BookManagementSystemDbContext _dbContext;
        private readonly JwtOptions _jwtOptions;

        public JwtService(BookManagementSystemDbContext dbContext, IOptions<JwtOptions> options)
        {
            _dbContext = dbContext;
            _jwtOptions = options.Value;
        }


        public async Task<TokenResult> GenerateToken(string username, List<string> roles)
        {
            // 從資料庫中查找對應的使用者
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // 建立 JWT Token 的 Claims，包含使用者名稱與jti
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            // 加入使用者角色
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 建立對稱加密金鑰，使用設定中的密鑰來編碼
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SignKey));

            // 使用 HMAC-SHA256 進行簽名
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer, // 發行者
                IssuedAt = DateTime.Now, // 發行時間
                Subject = new ClaimsIdentity(claims), // 使用者
                Expires = DateTime.Now.AddMinutes(_jwtOptions.ExpireMinutes),// 過期時間
                SigningCredentials = signingCredentials
            };

            // 建立 JWT Token
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtTokenHandler.CreateToken(tokenDescriptor);
            var token = jwtTokenHandler.WriteToken(securityToken); // accesstoken

            // 生成並儲存新的 Refresh Token
            var refreshToken = new RefreshToken()
            {
                JwtId = securityToken.Id,
                UserId = user.UserId,
                CreationTime = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMonths(6), // 有效期限為 6 個月
                Token = GenerateRandomNumber()  // 生成隨機的 Refresh Token
            };

            // 將 Refresh Token 存入資料庫
            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();

            // 返回生成的 Token 資訊
            return new TokenResult()
            {
                AccessToken = token,             // Access Token
                TokenType = "Bearer",            // Token 類型
                RefreshToken = refreshToken.Token,  // Refresh Token
                ExpireMinutes = (int)_jwtOptions.ExpireMinutes  // Token 過期時間（分鐘）
            };
        }
        public async Task<TokenResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var claimsPrincipal = GetClaimsPrincipalByToken(token); // 解析token，檢查其是否有效
            if (claimsPrincipal == null)
            {
                // 無效的token...
                return new TokenResult()
                {
                    Errors = new[] { "1: Invalid request!" },
                };
            }

            var expiryDateUnix = long.Parse(claimsPrincipal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = UnixTimeStampToDateTime(expiryDateUnix); // 取得 token 的過期時間並轉換為 DateTime
            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                // token未過期...
                return new TokenResult()
                {
                    Errors = new[] { "2: Invalid request!" },
                };
            }

            var jti = claimsPrincipal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _dbContext.RefreshTokens
                .SingleOrDefaultAsync(x => x.Token == refreshToken);
            if (storedRefreshToken == null)
            {
                // 無效的refresh_token...
                return new TokenResult()
                {
                    Errors = new[] { "3: Invalid request!" },
                };
            }

            if (storedRefreshToken.ExpiryTime < DateTime.UtcNow)
            {
                // refresh_token已過期...
                return new TokenResult()
                {
                    Errors = new[] { "4: Invalid request!" },
                };
            }

            if (storedRefreshToken.Invalidated)
            {
                // refresh_token已失效...
                return new TokenResult()
                {
                    Errors = new[] { "5: Invalid request!" },
                };
            }

            if (storedRefreshToken.Used)
            {
                // refresh_token已使用...
                return new TokenResult()
                {
                    Errors = new[] { "6: Invalid request!" },
                };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                // refresh_token與此token不匹配...
                return new TokenResult()
                {
                    Errors = new[] { "7: Invalid request!" },
                };
            }

            storedRefreshToken.Used = true;
            await _dbContext.SaveChangesAsync();

            var user = await _dbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == storedRefreshToken.UserId);

            var roles = user.Roles.Select(r => r.Name).ToList();
            return await GenerateToken(user.Username, roles);
        }

        private ClaimsPrincipal GetClaimsPrincipalByToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.Name,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SignKey)),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false, // 不驗證過期時間！！！
                    ValidIssuer = _jwtOptions.Issuer,
                };

                var jwtTokenHandler = new JwtSecurityTokenHandler();

                var claimsPrincipal =
                    jwtTokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                var validatedSecurityAlgorithm = validatedToken is JwtSecurityToken jwtSecurityToken
                                                 && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                                     StringComparison.InvariantCultureIgnoreCase);

                return validatedSecurityAlgorithm ? claimsPrincipal : null;
            }
            catch
            {
                return null;
            }
        }

        // 標記 Refresh Token 為失效
        public async Task<bool> InvalidateTokenAsync(string refreshtoken)
        {
            var refreshToken = await _dbContext.RefreshTokens
           .FirstOrDefaultAsync(rt => rt.Token == refreshtoken);

            if (refreshToken == null)
            {
                return false;
            }

            refreshToken.Invalidated = true;
            await _dbContext.SaveChangesAsync();

            return true;
        }


        private string GenerateRandomNumber()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }

    }
}

