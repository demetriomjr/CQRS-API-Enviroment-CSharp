public record UserCredentials(string username, string password);
public record JwtSettings(string issuer, string audience, string secretKey);