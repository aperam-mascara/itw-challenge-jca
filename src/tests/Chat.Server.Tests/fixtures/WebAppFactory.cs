using Chat.Server.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Chat.Server.Tests.fixtures;



/// <summary>
/// Web Application Factory for testing
/// </summary>
public class WebAppFactory: WebApplicationFactory<Program>, IAsyncLifetime
{

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("chatdb")
        .WithUsername("chatuser")
        .WithPassword("chatpassword")
        .Build();

    /// <summary>
    /// Web Application Factory for testing
    /// </summary>
    public WebAppFactory()
    {
        

    }


    



    /// <summary>
    /// Override the CreateHostBuilder method to configure the web host for testing.
    /// </summary>
    /// <param name="builder"></param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        
        IConfiguration configuration = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", false, true)
                       .Build();

        builder
        .UseConfiguration(configuration)
        
        .ConfigureTestServices(services =>
        {
            
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ChatDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ChatDbContext>(options =>
            {
                
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
           
        })
        
        ;
    }
    #region IAsyncLifetime
    /// <summary>
    /// Start the database container before running tests.
    /// </summary>
    /// <returns></returns>
    public Task InitializeAsync()
    =>_dbContainer.StartAsync();

    /// <summary>
    /// Stop the database container after running tests.
    /// </summary>
    /// <returns></returns>
    public new Task DisposeAsync()
    => _dbContainer.StopAsync();
    #endregion
}
