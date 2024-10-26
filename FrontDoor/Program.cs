using FrontDoor.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Properties"))
                                                        .AddJsonFile("appsettings.json", false, true);
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JWTSettings>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings?.Issuer,
            ValidAudience = jwtSettings?.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.SecretKey))
        };
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();

//AUTH ROUTES
app.MapPost("/token/create", (JWTSettings jwtSettings) =>
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, "usuário"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: jwtSettings.Issuer,
        audience: jwtSettings.Audience,
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: creds
    );

    return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
});

app.MapGet("/token/refresh", (string token) =>
{
    var handler = new JwtSecurityTokenHandler();
    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

    return jsonToken != null ? Results.Ok("Token válido") : Results.Unauthorized();
});


//PUBLIC ROUTES
app.MapGet("/{*path}", (string? path) =>
{
    return Results.Ok();
}).RequireAuthorization();

app.MapPost("/{*path}", (string? path) =>
{
    return Results.Ok();
}).RequireAuthorization();

app.MapPut("/{*path}", (string? path) =>
{
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/{*path}", (string? path) =>
{
    return Results.Ok();
}).RequireAuthorization();