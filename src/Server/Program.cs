using Chat.Server.data;
using Microsoft.EntityFrameworkCore;
using EFUtilities;
var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ChatDbConnection")));

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

// Apply migrations
app.MigrateDatabase<ChatDbContext>();

app.MapGet("/", () => "Server Running");

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();
