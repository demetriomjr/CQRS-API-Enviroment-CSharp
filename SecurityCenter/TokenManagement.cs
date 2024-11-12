using Microsoft.EntityFrameworkCore;
using Models;

namespace SecurityHQ
{
    public class TokenManagement
    {
        private static readonly int TOKEN_DURATION = 30;
        private static readonly int REFRESH_TOKEN_DURATION = 60 * 12;

        public TokenResponse GenerateToken(JwtSettings settings, Guid userId)
        {
            var userCode = Guid.NewGuid().ToString().Replace("-", "");

            //function to set userId with userCode on Redis

            var token = CreateToken(userCode, settings!, TOKEN_DURATION);
            var refreshToken = CreateToken(userCode, settings!, REFRESH_TOKEN_DURATION);

            return new(
                userCode,
                new JwtSecurityTokenHandler().WriteToken(token),
                new JwtSecurityTokenHandler().WriteToken(refreshToken)
            );
        }

        public TokenResponse RefreshToken(JwtSettings settings, string refreshToken)
        {
            var userCode = RecoverUserCode(refreshToken, settings, out string error);

            if(userCode is null)
                return new(null!, null!, null!, error);

            var token = CreateToken(userCode, settings!, TOKEN_DURATION);
            var newRefreshToken = CreateToken(userCode, settings!, REFRESH_TOKEN_DURATION);

            return new(
                userCode,
                new JwtSecurityTokenHandler().WriteToken(token),
                new JwtSecurityTokenHandler().WriteToken(newRefreshToken)
            );
        }

        public bool IsValidUsageToken(string jwtoken)
        {
            return true;
        }

        private JwtSecurityToken CreateToken(string userCode, JwtSettings settings, int duration)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userCode),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserCode", userCode)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: settings.issuer,
                audience: settings.audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(duration),
                signingCredentials: creds
            );
        }

        private string RecoverUserCode(string token, JwtSettings settings, out string error)
        {
            error = "";
            try
            {
                var key = Encoding.UTF8.GetBytes(settings.secretKey);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = settings.issuer,
                    ValidAudience = settings.audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = false
                };

                var claims = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);

                if (claims is null)
                {
                    error = "An error has occurred while trying to retrieve the token\'s claims.";
                    return null!;
                }

                var userCode = claims.FindFirst("UserCode")?.Value!;

                if(userCode is null)
                {
                    error = "UserCode was not found in the token\'s claims.";
                    return null!;
                }

                return userCode;
            }
            catch (Exception ex)
            {
                if(Debugger.IsAttached)
                    throw;
                error = ex.Message;
                return null!;
            }  
        }

        private async Task<(bool completed, string error)> BlackListToken(JwtResponse jwtResponse)
        {
            try
            {
                using (var connection = Database.Redis)
                {
                    var db = connection.Connect();
                    var blackListedToken = await db.StringGetAsync($"blacklist:{jwtResponse.userCode}");

                    if (blackListedToken.HasValue)
                        return (true, "token already black-listed");

                    await db.HashSetAsync($"blacklist:{jwtResponse.userCode}",
                    [
                        new("token", jwtResponse.token),
                        new("expiry", jwtResponse.expiration.ToString("o"))
                    ]);

                    await db.KeyExpireAsync(jwtResponse.userCode, jwtResponse.expiration.AddSeconds(90));

                    return (true, string.Empty);
                }
            }
            catch(Exception ex)
            {
                if(Debugger.IsAttached) throw;
                return (false, ex.Message);
            }
        }

        private async Task<bool> IsBlackListedToken(string key, string token)
        {
            try
            {
                using(var connection = Database.Redis)
                {
                    var db = connection.Connect();
                    var readableKey = $"blacklist:{key}";
                    var reuslt = await db.HashGetAsync(readableKey, "token");
                    return token.Equals(token);
                }
            }
            catch (Exception)
            {
                if(Debugger.IsAttached)
                    throw;
                return false;
            }
        }
    }
}
