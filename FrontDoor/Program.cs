int TOKEN_DURATION = 30;
int REFRESH_TOKEN_DURATION = 60*12;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Properties")).AddJsonFile("appsettings.json", false, true);
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings?.issuer,
            ValidAudience = jwtSettings?.audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.secretKey))
        };
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();

//AUTH ROUTES
app.MapPost("/token/authorize", (UserCredentials credentials) =>
{
    if (credentials is null)
        return Results.BadRequest();

    if (credentials is null)
        return Results.Unauthorized();

    return Results.Ok(GenerateToken(credentials));
});

app.MapGet("/token/refresh", (string refresToken) =>
{
    var jsonToken = new JwtSecurityTokenHandler().ReadToken(refresToken) as JwtSecurityToken;

    if( jsonToken is null)
        return Results.Unauthorized();

    return Results.Ok(RefreshToken(refresToken));
});

//PUBLIC ROUTES
app.MapGet("/{*path}", (string? path) =>
{
    if (path is null)
        return Results.NotFound();

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

JwtSecurityToken CreateToken(UserCredentials user, JwtSettings jwtSettings, int duration)
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

(string token, string refresh_token) GenerateToken(UserCredentials credentials)
{
    
    var token = CreateToken(credentials, jwtSettings!, TOKEN_DURATION);
    var refresh_t = CreateToken(credentials, jwtSettings!, REFRESH_TOKEN_DURATION);

    return (
            new JwtSecurityTokenHandler().WriteToken(token),
            new JwtSecurityTokenHandler().WriteToken(refresh_t)
           );
}

(string token, string refresh_token) RefreshToken(string refreshToken)
{
    var credentials = new UserCredentials("", "");
    var token = CreateToken(credentials, jwtSettings!, TOKEN_DURATION);
    var newRefreshToken = CreateToken(credentials, jwtSettings!, REFRESH_TOKEN_DURATION);

    return (
            new JwtSecurityTokenHandler().WriteToken(token),
            new JwtSecurityTokenHandler().WriteToken(newRefreshToken)
           );
}