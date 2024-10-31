var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Run("locahost:5001/users");


app.MapGet("/{*id}", (string? id, HttpContext context) =>
{

});

app.MapPost("/", (HttpContext context) =>
{

});

app.MapPut("/{*id}", (string id, HttpContext context) =>
{

});

app.MapDelete("/{*id}", (string id) =>
{

});