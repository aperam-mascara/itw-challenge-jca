using Chat.Server.data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace Chat.Server.Tests;



/// <summary>
/// Web Application Factory for testing
/// </summary>
public class WebAppFactory(): WebApplicationFactory<Program>, IAsyncLifetime
{

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("chatdb")
        .WithUsername("chatuser")
        .WithPassword("chatpassword")
        .Build();

    /// <summary>
    /// Test Output Helper
    /// </summary>
    private ITestOutputHelper? _output;

    /// <summary>
    /// Test Output Helper
    /// </summary>
    public ITestOutputHelper OutputHelper
    {
        get
        {
            return _output!;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(OutputHelper));
            _output = value;
        }
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
                options.LogTo(m => OutputHelper.WriteLine(m), LogLevel.Information);
                
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
           /* var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

                // Ensure the database is created
                dbContext.Database.EnsureCreated();
            }*/
        }).ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.AddXUnitLogging(configuration, () => OutputHelper);
        });
    }
    #region IAsyncLifetime
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task InitializeAsync()
    =>_dbContainer.StartAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new Task DisposeAsync()
    => _dbContainer.StopAsync();
    #endregion
}
