using UserServices;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Run(Path.Combine("localhost:5001", "users"));


app.MapGet("/{*value}", (string? value, HttpContext context) =>
{

});

app.MapGet("/validate", async ([FromQuery] string username, [FromQuery] string password) =>
{
    var result = await new UsersManagement().ValidateAndGenerateCode(username, password);
    return result;
});

app.MapPost("/", (HttpContext context) =>
{

});

app.MapPut("/{value}", (string? value, HttpContext context) =>
{

});

app.MapDelete("/{value}", (string? value) =>
{

});