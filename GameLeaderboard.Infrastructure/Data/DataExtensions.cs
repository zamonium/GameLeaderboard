using System.Text;
using GameLeaderboard.Domain.Entities;
using GameLeaderboard.Infrastructure.Services;
using GameLeaderboard.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace GameLeaderboard.Infrastructure.Data;

public static class DataExtensions
{
    public static void MigrateDb(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameLeaderboardContext>();
        dbContext.Database.Migrate();
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config,
        Action<DbContextOptionsBuilder>? dbContextOptions = null)
    {
        var jwtSection = config.GetSection("Jwt");

        services.Configure<JwtSettings>(jwtSection);
        if (dbContextOptions is not null)
            services.AddDbContext<GameLeaderboardContext>(dbContextOptions);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSection.Get<JwtSettings>()!.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        return services;
    }
}
