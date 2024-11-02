var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Run("localhost:5001/users");


app.MapGet("/{*value}", (string? value, HttpContext context) =>
{

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