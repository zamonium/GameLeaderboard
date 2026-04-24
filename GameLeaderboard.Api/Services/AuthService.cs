using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameLeaderboard.Api.Data;
using GameLeaderboard.Api.DTOs.Auth;
using GameLeaderboard.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameLeaderboard.Api.Services;

public class AuthService : IAuthService
{
    private readonly GameLeaderboardContext dbContext;
    private readonly IConfiguration config;
    private readonly IPasswordHasher<User> passwordHasher;

    public AuthService(GameLeaderboardContext dbContext, IConfiguration config, IPasswordHasher<User> passwordHasher)
    {
        this.dbContext = dbContext;
        this.config = config;
        this.passwordHasher = passwordHasher;
    }
    
    public async Task<RegisterResult> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var exist = await dbContext.Users.AnyAsync(user => user.Email == request.Email, ct);

        if(exist)
        {
            return RegisterResult.Fail("Email already in use");
        }

        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
        };
        user.SetPassword(request.Password, passwordHasher);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        return RegisterResult.Ok();
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == request.Email, ct);

        if(user == null)
        {
            return LoginResult.Fail("Invalid credentials");
        }

        var passwordVerification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if(passwordVerification == PasswordVerificationResult.Failed)
        {
            return LoginResult.Fail("Invalid credentials");
        }

        var token = GenerateToken(user);

        return LoginResult.Ok(token);
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Secret"]!));
        int expirationHours = config.GetValue<int>("Jwt:ExpiryHours");

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddHours(expirationHours),
            claims: claims,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
