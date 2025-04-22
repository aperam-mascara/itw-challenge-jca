using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;
using Xunit.Abstractions;

namespace XUnitPostgreSQL;

/// <summary>
/// PostgreSQL Fixture for testing
/// It is not intended to be used directly. Use in test class with implementing @see IClassFixture instead.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PostgreSQLFixture<T> : IAsyncLifetime, IDisposable where T : DbContext
{


    
    private readonly PostgreSqlContainer _dbContainer;
    /// <summary>
    /// Service Provider
    /// </summary>
    public readonly ServiceProvider ServiceProvider;
    private bool disposedValue;
    /// <summary>
    /// Test Output Helper
    /// </summary>
    private ITestOutputHelper? _output;

    /// <summary>
    /// PostgreSQL Fixture. It is not intended to be used directly. Use in test class with implementing IClassFixture
    /// </summary>
    public PostgreSQLFixture()
    {



        IConfiguration configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", false, true)
                   .Build();
        var options = configuration.GetSection("PostgreSQLContainer").Get<PostgreSQLFixtureOptions>();
        options ??= new PostgreSQLFixtureOptions();
        options.Validate();
        _dbContainer = new PostgreSqlBuilder()
            .WithImage(options.ImageName)
            .WithDatabase(options.DatabaseName)
            .WithUsername(options.Username)
            .WithPassword(options.Password)
            .Build();

        this.ServiceProvider = new ServiceCollection()
        .AddDbContext<T>(options =>
        {

            options.UseNpgsql(_dbContainer.GetConnectionString(), b =>
            {
                b.MigrationsAssembly(typeof(T).Assembly.FullName);

            });
            options.LogTo(m => _output?.WriteLine(m), LogLevel.Information);
        }, ServiceLifetime.Transient)
        .AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddXUnitLogging(configuration, () => OutputHelper);

        })
        .BuildServiceProvider();

        _dbContainer.StartAsync().Wait();

        T ctx = ServiceProvider.GetRequiredService<T>();


        //apply migrations to ensure the database is up to date
        ctx.Database.Migrate();

    }



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
    /// Get the DbContext
    /// </summary>
    /// <param name="tablesToClear">table name to truncate when getting a new DbContext</param>
    /// <returns></returns>
    public async Task<T> ContextAsync(IEnumerable<string>? tablesToClear = default)
    {
        var ctx = ServiceProvider.GetRequiredService<T>();
        var logger = ServiceProvider.GetService<ILogger<PostgreSQLFixture<T>>>();
        if (tablesToClear?.Any() ?? false)
        {
            string tableNames = string.Join(", ", tablesToClear.Select(tableName => @$"public.""{tableName}"""));
            await ctx.Database.OpenConnectionAsync();
            string sql = $"TRUNCATE {tableNames} RESTART IDENTITY CASCADE;";
            using var command = new NpgsqlCommand(sql, (NpgsqlConnection)ctx.Database.GetDbConnection());
            command.ExecuteNonQuery();
            logger?.LogInformation("Tables {tableNames} have been successfully truncated !",tableNames);
        }


        return ctx;
    }
    #region IDisposable
    /// <summary>
    /// Dispose
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: supprimer l'état managé (objets managés)
                //                    _dbContainer.StopAsync().Wait();    
            }

            // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
            // TODO: affecter aux grands champs une valeur null
            disposedValue = true;
        }
    }

    // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
    // ~PostgreSQLFixture()
    // {
    //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
    //     Dispose(disposing: false);
    // }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    #region IAsyncLifetime
    /// <summary>
    /// DisposeAsync
    /// </summary>
    /// <returns></returns>
    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
    /// <summary>
    /// InitializeAsync
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

    }
    #endregion
}
