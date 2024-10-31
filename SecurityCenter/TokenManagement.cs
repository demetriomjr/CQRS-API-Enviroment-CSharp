namespace SecurityHQ
{
    public class TokenManagement
    {
        private static readonly int TOKEN_DURATION = 30;
        private static readonly int REFRESH_TOKEN_DURATION = 60 * 12;

        public JwtSecurityToken CreateToken(UserCredentials credentials, JwtSettings jwtSettings, int duration)
        {
            var user = new User();
            var userCode = Guid.NewGuid().ToString();

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, userCode)
    };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: jwtSettings.issuer,
                audience: jwtSettings.audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(duration),
                signingCredentials: creds
            );
        }

        public (string token, string refresh_token) GenerateToken(JwtSettings jwtSettings, UserCredentials credentials)
        {

            var token = CreateToken(credentials, jwtSettings!, TOKEN_DURATION);
            var refresh_t = CreateToken(credentials, jwtSettings!, REFRESH_TOKEN_DURATION);

            return (
                    new JwtSecurityTokenHandler().WriteToken(token),
                    new JwtSecurityTokenHandler().WriteToken(refresh_t)
                   );
        }

        public (string token, string refresh_token) RefreshToken(JwtSettings jwtSettings, string refreshToken)
        {
            var credentials = new UserCredentials("", "");
            var token = CreateToken(credentials, jwtSettings!, TOKEN_DURATION);
            var newRefreshToken = CreateToken(credentials, jwtSettings!, REFRESH_TOKEN_DURATION);

            return (
                    new JwtSecurityTokenHandler().WriteToken(token),
                    new JwtSecurityTokenHandler().WriteToken(newRefreshToken)
                   );
        }
    }
}
