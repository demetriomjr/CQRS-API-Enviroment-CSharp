namespace Models
{
    public record UserValidationResponse(bool isValid, Guid userId, string error);
    public record JwtResponse(string userCode, string token, DateTime expiry);
    public record JwtSettings(string issuer, string audience, string secretKey);
    public record TokenResponse(string userCode, string token, string refreshToken, string error = null!);
}