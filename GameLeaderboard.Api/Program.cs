using GameLeaderboard.Api.Exceptions;
using GameLeaderboard.Infrastructure.Data;
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
    var connString = builder.Configuration.GetConnectionString("GameLeaderboard");
    builder.Services.AddInfrastructure(builder.Configuration, connString!);

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddAuthorization();

    builder.Services.AddControllers();

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

    app.Services.MigrateDb();

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
