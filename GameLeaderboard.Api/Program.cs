using FluentValidation;
using FluentValidation.AspNetCore;
using GameLeaderboard.Api.Exceptions;
using GameLeaderboard.Infrastructure.Data;
using GameLeaderboard.Infrastructure.Validators.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Generation.Processors.Security;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting GameLeaderboard API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services));

    // Add services to the container.
    if (!builder.Environment.IsEnvironment("Testing"))
    {
        var connString = builder.Configuration.GetConnectionString("GameLeaderboard");
        builder.Services.AddInfrastructure(builder.Configuration,
            options => options.UseSqlServer(connString));
    }
    else
    {
        builder.Services.AddInfrastructure(builder.Configuration);
    }

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddAuthorization();

    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred",
                    Instance = context.HttpContext.Request.Path
                };

                return new BadRequestObjectResult(problemDetails);
            };
        });

    builder.Services.AddOpenApiDocument(options =>
    {
        options.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
        {
            Type         = NSwag.OpenApiSecuritySchemeType.Http,
            Scheme       = "bearer",
            BearerFormat = "JWT",
            Description  = "Enter your JWT token."
        });
        options.OperationProcessors.Add(
            new AspNetCoreOperationSecurityScopeProcessor("Bearer")
        );
    });

    builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
    builder.Services.AddFluentValidationAutoValidation();

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = 
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseOpenApi();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseHttpsRedirection();
    }

    app.UseExceptionHandler();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    if (!app.Environment.IsEnvironment("Testing"))
    {
        app.Services.MigrateDb();
    }

    app.Run();
}
catch(Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
