using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameLeaderboard.Infrastructure.Data;
using GameLeaderboard.Infrastructure.DTOs.Auth;
using GameLeaderboard.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using GameLeaderboard.Infrastructure.Settings;
using Microsoft.Extensions.Logging;

namespace GameLeaderboard.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly GameLeaderboardContext dbContext;
    private readonly IOptions<JwtSettings> jwtOptions;
    private readonly IPasswordHasher<User> passwordHasher;
    private readonly ILogger<AuthService> logger;

    public AuthService(GameLeaderboardContext dbContext, 
        IOptions<JwtSettings> jwtOptions, 
        IPasswordHasher<User> passwordHasher,
        ILogger<AuthService> logger)
    {
        this.dbContext = dbContext;
        this.jwtOptions = jwtOptions;
        this.passwordHasher = passwordHasher;
        this.logger = logger;
    }
    
    public async Task<RegisterResult> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var exist = await dbContext.Users.AnyAsync(user => user.Email == request.Email, ct);

        if(exist)
        {
            logger.LogWarning("Registration failed for {Email} - email already in use", request.Email);
            return RegisterResult.Fail("Email already in use");
        }

        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {Email} registered successfully", request.Email);
        return RegisterResult.Ok();
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == request.Email, ct);

        if(user == null)
        {
            logger.LogWarning("Login failed for {Email} - user not found", request.Email);
            return LoginResult.Fail("Invalid credentials");
        }

        var passwordVerification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if(passwordVerification == PasswordVerificationResult.Failed)
        {
            logger.LogWarning("Login failed for {Email} - wrong password", request.Email);
            return LoginResult.Fail("Invalid credentials");
        }

        var token = GenerateToken(user);

        logger.LogInformation("User {Email} logged in successfully", request.Email);
        return LoginResult.Ok(token);
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Value.Secret));
        int expirationHours = jwtOptions.Value.ExpiryHours;

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddHours(expirationHours),
            claims: claims,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
