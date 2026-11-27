using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=volly.db"));

var app = builder.Build();

var passwordHasher = new PasswordHasher<User>();

app.MapGet("/", () => "Volly server is running");

// Регистрация
app.MapPost("/signup", async (SignupRequest request, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) ||
        string.IsNullOrWhiteSpace(request.Email) ||
        string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "Please fill in all fields"
        });
    }

    string email = request.Email.Trim();

    bool emailExists = await db.Users
        .AnyAsync(user => user.Email == email);

    if (emailExists)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "This email is already registered"
        });
    }

    var user = new User
    {
        Name = request.Name.Trim(),
        Email = email,
        Role = request.Role,
        Password = ""
    };

    // Сохраняем не сам пароль, а его хеш
    user.Password = passwordHasher.HashPassword(user, request.Password);

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        success = true,
        message = "Registration successful",
        user = new
        {
            user.Id,
            user.Name,
            user.Email,
            user.Role
        }
    });
});

// Вход
app.MapPost("/login", async (LoginRequest request, AppDbContext db) =>
{
    string email = request.Email.Trim();

    var user = await db.Users
        .FirstOrDefaultAsync(user => user.Email == email);

    if (user == null)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "Incorrect email or password"
        });
    }

    // Поддержка аккаунтов, созданных до включения хеширования
    if (user.Password == request.Password)
    {
        user.Password = passwordHasher.HashPassword(user, request.Password);
        await db.SaveChangesAsync();
    }
    else
    {
        var verificationResult = passwordHasher.VerifyHashedPassword(
            user,
            user.Password,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Incorrect email or password"
            });
        }

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.Password = passwordHasher.HashPassword(user, request.Password);
            await db.SaveChangesAsync();
        }
    }

    return Results.Ok(new
    {
        success = true,
        message = "Login successful",
        user = new
        {
            user.Id,
            user.Name,
            user.Email,
            user.Role
        }
    });
});

app.Run();

public class SignupRequest
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "Player";
}

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}