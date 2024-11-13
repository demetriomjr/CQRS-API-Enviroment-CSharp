namespace Models
{
    public record UserValidationResponse(bool isValid, Guid userId, string error);
    public record JwtResponse(string userCode, string token, DateTime expiry);
}