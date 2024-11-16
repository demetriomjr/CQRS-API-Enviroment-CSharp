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

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

var usersQueue = (await channel.QueueDeclareAsync(queue: "users",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null)).QueueName;

//AUTH ROUTES
app.MapGet("/token/authorize", async ([FromQuery] string username, [FromQuery] string password) =>
{
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        return Results.BadRequest("Username and Password are required to validate.");

    var result = new { };

    /*REWORK*/
    await Task.Delay(1000);
    var validation = new UserValidationResponse(false, Guid.Empty, string.Empty);

    if (validation is null)
        return Results.BadRequest();

    if (validation.isValid)
        return Results.Ok(/*generate token*/);

    return Results.Unauthorized();
});

app.MapGet("/token/refresh", ([FromQuery] string refresToken) =>
{
    if (refresToken is null)
        return Results.BadRequest("No Refresh Token was passed as parameter");

    var jsonToken = new JwtSecurityTokenHandler().ReadToken(refresToken) as JwtSecurityToken;

    if (jsonToken is null || jsonToken.ValidTo <= DateTime.UtcNow)
        return Results.Unauthorized();

    return Results.Ok(/*REFRESH TOKEN HERE*/);
});

app.MapPost("/users/register", async ([FromBody] User user) =>
{
    await Task.Delay(100);
    return Results.Forbid();
});

//PUBLIC ROUTES
app.MapGet("/{*path}", async (string? path) =>
{
    if (path is null)
        return Results.NotFound();

    /*REWORK*/
    await Task.Delay(100);
    var result = new TokenResponse("", "", "");

    if (result is null)
        return Results.BadRequest();

    return Results.Ok(result);
}).RequireAuthorization();

app.MapPost("/{*path}", async (string? path, [FromBody] HttpContent content) =>
{
    if (path is null)
        return Results.NotFound();

    /*REWORK*/
    await Task.Delay(100);
    var result = new TokenResponse("", "", "");

    if (result is null)
        return Results.BadRequest();

    return Results.Ok(result);
}).RequireAuthorization();

app.MapPut("/{*path}", async (string? path, [FromBody] HttpContent content) =>
{
    if (path is null)
        return Results.BadRequest();

    /*REWORK*/
    await Task.Delay(100);
    var result = new TokenResponse("", "", "");

    if (result is null)
        return Results.NotFound("No result returned");

    return Results.Ok(result);
}).RequireAuthorization();

app.MapDelete("/{*path}", async (string? path) =>
{
    if (path is null)
        return Results.BadRequest();

    /*REWORK*/
    await Task.Delay(100);
    var result = new TokenResponse("", "", "");

    if (result is null)
        return Results.NotFound("No result returned");

    return Results.Ok(result);
}).RequireAuthorization();