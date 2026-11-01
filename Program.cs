var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/login", (string username, string password) =>
{
    if (username == "vova" && password == "227")
    {
        return Results.Ok("Login successful");
    }
    else
    {
        return Results.BadRequest("Invalid username or password");
    }
});

app.MapGet("/signup", (string username, string password) =>
{
    // Implementation for signup logic
    return Results.Ok("Signup successful");
});

app.Run();