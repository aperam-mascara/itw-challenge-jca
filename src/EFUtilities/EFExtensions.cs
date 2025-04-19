using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EFUtilities;

/// <summary>
/// This class contains extension methods for Entity Framework Core.
/// </summary>
public static class EFExtensions
{
    /// <summary>
    /// Extension method to apply migrations to the database.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="host"></param>
    /// <returns></returns>
    public static IHost MigrateDatabase<TDbContext>(this IHost host) where TDbContext : DbContext
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<TDbContext>();
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<TDbContext>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }
        return host;
    }

    
    /// <summary>
    /// Extension method to seed the database with initial data.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="app"></param>
    /// <param name="seedAction"></param>
    /// <returns></returns>
    public static async Task<IHost> SeedAsync<TDbContext>(this IHost app, Func<TDbContext, Task> seedAction)
        where TDbContext : DbContext
    {
        
        using (var scope = app.Services.CreateScope())
        {
            
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<TDbContext>();

                // Assurez-vous que la base de données est créée
                await dbContext.Database.EnsureCreatedAsync();

                // Exécutez l'action de seeding fournie
                await seedAction(dbContext);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<TDbContext>>();
                logger.LogError(ex, "Une erreur s'est produite lors du seeding de la base de données.");
                throw;
            }
        }

        return app;
    }
}
