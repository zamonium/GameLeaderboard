using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameLeaderboard.Api.DTOs;
using GameLeaderboard.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace GameLeaderboard.Api.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration config;

    public AuthService(IConfiguration config)
    {
        this.config = config;
    }
    
    public async Task<RegisterResult> RegisterAsync(RegisterRequest request)
    {
        //check if user exists

        //hash password

        //create and save user

        return RegisterResult.Ok();
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request)
    {
        //find user

        //verify password

        //generate token
        User user = new User(){ Id = 1, Email = request.Email};
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
