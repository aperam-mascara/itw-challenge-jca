using Chat.Routes;
using Chat.Server.data;
using Chat.Shared.Models;
using EFUtilities;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.RegularExpressions;
var builder = WebApplication.CreateBuilder(args);



builder.Services.Configure<ChatRouteOptions>(builder.Configuration.GetSection(ChatRouteOptions.SECTION_NAME));
// Add health checks
builder.Services.AddHealthChecks();

// Add CORS
builder.Services.AddCors(options =>
{

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add DbContext
builder.Services.AddDbContext<ChatDbContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("ChatDbConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'ChatDbConnection' not found.");
    }
#if DEBUG
    bool isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!isRunningInDocker)
    {
        connectionString = ConnectionStringReplaceHostName().Replace(connectionString, "Host=localhost");
    }
#endif
    options.UseNpgsql(connectionString);
});

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.UseCors("AllowAll");


if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithDarkMode(true).WithTitle("Simple Chat API");
    });
#if DEBUG    
    //Allow to seed fake data for testing
    app.MapGet("/api/seed", async () =>
    {
        await app.SeedAsync<ChatDbContext>(async dbContext =>
        {
            
            // Seed the database with initial data
            if (!await dbContext.Users.AnyAsync())
            {
                List<User> users = [
                    new() { Username = "Alice" },
                    new() { Username = "Bob" },
                    new() { Username = "Charlie" }
                ];
                await dbContext.Users.AddRangeAsync(users);
                await dbContext.SaveChangesAsync();
            }
        });
        return Results.Ok("Database seeded");
    });
#endif
}

// Apply migrations
app.MigrateDatabase<ChatDbContext>();

// Configure Chat Routes
app.ConfigureChatRoutes<ChatDbContext>();

app.MapGet("/", () => Results.Ok("Server Running ...."));


//app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.MapHealthChecks("/health");
app.Run();


//Do Not remove this line, it is used by the tests to get the entry point
#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
public partial class Program;
partial class Program
{
#if DEBUG
    [GeneratedRegex(@"Host=([^;]+)")]
    public static partial Regex ConnectionStringReplaceHostName();
#endif
}
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement