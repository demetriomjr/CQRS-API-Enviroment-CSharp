namespace Models
{
    public record UserValidationResponse(bool isValid, Guid userCode, string error);
    public record JwtResponse(string userCode, string token, DateTime expiration);
}