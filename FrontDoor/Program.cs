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
app.MapGet("/token/authorize", ([FromQuery] string username, [FromQuery] string password) =>
{
    if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        return Results.BadRequest("Username and Password are required to validate.");

    if (SecurityCenter.Users.Validate(username, password, out Guid userCode))
        return Results.Ok(SecurityCenter.Tokens.GenerateToken(jwtSettings!, userCode));

    return Results.Unauthorized();
});

app.MapGet("/token/refresh", ([FromQuery]string refresToken) =>
{
    if (refresToken is null)
        return Results.BadRequest("No Refresh Token was passed as parameter");

    var jsonToken = new JwtSecurityTokenHandler().ReadToken(refresToken) as JwtSecurityToken;

    if (jsonToken is null || jsonToken.ValidTo <= DateTime.UtcNow)
        return Results.Unauthorized();

    return Results.Ok(SecurityCenter.Tokens.RefreshToken(jwtSettings!, refresToken));
});

app.MapPost("/users/register", async ([FromBody] User user) =>
{
    
});

//PUBLIC ROUTES
app.MapGet("/{*path}", async (string? path) =>
{
    if (path is null)
        return Results.NotFound();

    var result = await Microservices.Get(path);

    if (result is null)
        return Results.BadRequest();

    return Results.Ok(result);
}).RequireAuthorization();

app.MapPost("/{*path}", async (string? path, [FromBody] HttpContent  content) =>
{
    if (path is null)
        return Results.NotFound();

    var result = await Microservices.Post(path, content);

    if (result is null)
        return Results.BadRequest();

    return Results.Ok(result);
}).RequireAuthorization();

app.MapPut("/{*path}", async (string? path, [FromBody] HttpContent content) =>
{
    if (path is null)
        return Results.NotFound();

    var result = await Microservices.Put(path, content);

    if (result is null)
        return Results.BadRequest();

    return Results.Ok(result);
}).RequireAuthorization();

app.MapDelete("/{*path}", async (string? path) =>
{
    if (path is null)
        return Results.NotFound();

    var result = await Microservices.Delete(path);

    if (result is null)
        return Results.BadRequest();

    return Results.Ok(result);
}).RequireAuthorization();