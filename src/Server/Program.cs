using Chat.Routes;
using Chat.Server.data;
using Chat.Shared.Models;
using EFUtilities;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ChatDbConnection")));


builder.Services.Configure<ChatRouteOptions>(builder.Configuration.GetSection(ChatRouteOptions.SECTION_NAME));

WebApplication app = builder.Build();




if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
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


app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();
