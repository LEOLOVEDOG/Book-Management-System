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
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SignKey));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(_jwtOptions.ExpireMinutes),
                SigningCredentials = signingCredentials
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtTokenHandler.CreateToken(tokenDescriptor);
            var token = jwtTokenHandler.WriteToken(securityToken);

            var refreshToken = new RefreshToken()
            {
                JwtId = securityToken.Id,
                UserId = user.UserId,
                CreationTime = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMonths(6),
                Token = GenerateRandomNumber()
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new TokenResult()
            {
                AccessToken = token,
                TokenType = "Bearer",
                RefreshToken = refreshToken.Token,
                ExpireMinutes = (int)_jwtOptions.ExpireMinutes,
            };
        }

        public async Task<TokenResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var claimsPrincipal = GetClaimsPrincipalByToken(token);
            if (claimsPrincipal == null)
            {
                // 無效的token...
                return new TokenResult()
                {
                    Errors = new[] { "1: Invalid request!" },
                };
            }

            var expiryDateUnix =
                long.Parse(claimsPrincipal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = UnixTimeStampToDateTime(expiryDateUnix);
            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                // token未過期...
                return new TokenResult()
                {
                    Errors = new[] { "2: Invalid request!" },
                };
            }

            var jti = claimsPrincipal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken =
                await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
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
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SignKey)),
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false // 不驗證過期時間！！！
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

