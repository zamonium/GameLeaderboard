using GameLeaderboard.Api.Exceptions;
using GameLeaderboard.Infrastructure.Data;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

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
