global using Microsoft.IdentityModel.Tokens;
global using System.Diagnostics;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Text;
global using Models;
global using Databases;
global using StackExchange.Redis;

public record JwtSettings(string issuer, string audience, string secretKey);
public record TokenResponse(string userCode, string token, string refreshToken, string error = null!);