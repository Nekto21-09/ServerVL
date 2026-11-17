var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

User user = new User()
{
    Name = "Vova",
    Password = "123"
    Email = "Vova227@gmail.com"
};

app.MapGet("/login", (string username, string password, string email) =>
{
    if (password == user.Password && email == user.Email)
    {
        return Results.Ok(user);
    }
    else
    {
        return Results.BadRequest("Invalid username or password");
    }
});

app.MapGet("/signup", (string username, string password, string email) =>
{
    if (email == user.Email)
    {
        return Results.BadRequest("Username already taken");
    }
    else
    {
        User newuser = new User()
        {
            Name = username,
            Password = password,
            Email = email,
        };

        return Results.Ok(newuser);
    }
});

app.MapGet("/askai", (string request) =>
{
    // send request to gemini
});


app.Run();