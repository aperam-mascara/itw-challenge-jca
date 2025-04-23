using Chat.Server;
using Chat.Server.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);




// Vous pouvez configurer Serilog ici (console, fichier, etc.)
builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Add health checks
builder.Services.AddHealthChecks();


// Add DbContext
builder.Services.AddDbContext<ChatDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Apply migrations and ensure database
using (var scope = app.Services.CreateScope())
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

    logger.LogInformation(". Applying Migrations");
    db.Database.Migrate();
    logger.LogInformation("Migrations done ...");

}


if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithDarkMode(true).WithTitle("Simple Chat API");
    });
}


// Configure Chat Routes
app.ConfigureChatRoutes<ChatDbContext>();



app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.MapHealthChecks("/health");
app.Run();



//Do Not remove this line, it is used by the tests to get the entry point
#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
public partial class Program { }   
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement