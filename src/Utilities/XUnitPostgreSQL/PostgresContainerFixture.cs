using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace XUnitPostgreSQL;

/// <summary>
/// PostegreSQL Fixture for testing
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public class PostgresContainerFixture<TDbContext> : IAsyncLifetime where TDbContext:DbContext
{
    private readonly PostgreSqlContainer _dbContainer;

    /// <summary>
    /// PostgreSQL Fixture Configuration options
    /// </summary>
    public IConfiguration Configuration { get; private init; }

    
    private readonly ServiceProvider _serviceProvider ;

    /// <summary>
    /// PostgreSQL Fixture with PostgreSQL Docker container for testing
    /// </summary>
    public PostgresContainerFixture()
    {
     
        Configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", false, true)
               .Build();

        var options = Configuration.GetSection("PostgreSQLContainer").Get<PostgreSQLFixtureOptions>();
        options ??= new PostgreSQLFixtureOptions();
        options.Validate();
        _dbContainer = new PostgreSqlBuilder()
            .WithImage(options.ImageName)
            .WithDatabase(options.DatabaseName)
            .WithUsername(options.Username)
            .WithPassword(options.Password)
        .Build();


        _serviceProvider = new ServiceCollection()
         .AddDbContext<TDbContext>(options =>
         {

             options.UseNpgsql(_dbContainer.GetConnectionString(), b =>
             {
                 b.MigrationsAssembly(typeof(TDbContext).Assembly.FullName);

             });
         }, ServiceLifetime.Transient)
         
         .BuildServiceProvider();

    }


    /// <summary>
    /// Initialize the PostgreSQL container and apply migrations
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        // Démarrage du conteneur
        await _dbContainer.StartAsync();
        using TDbContext context =Create();

        await context.Database.MigrateAsync();
    }

    /// <summary>
    /// Create a new DbContext
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public TDbContext Create()
        =>_serviceProvider?.GetRequiredService<TDbContext>() ?? throw new ApplicationException("You must set ITestOutputHelper first by calling AddOutputHelper");



    /// <summary>
    /// Get a new DbContext and reset the tables
    /// </summary>
    /// <param name="tablesToClear">table name to truncate when getting a new DbContext</param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public async Task<TDbContext> ContextAndResetTablesAsync(IEnumerable<string>? tablesToClear = default, Serilog.Core.Logger? logger=default)
    {
        var ctx = Create();
       
        if (tablesToClear?.Any() ?? false)
        {
            string tableNames = string.Join(", ", tablesToClear.Select(tableName => @$"public.""{tableName}"""));
            await ctx.Database.OpenConnectionAsync();
            string sql = $"TRUNCATE {tableNames} RESTART IDENTITY CASCADE;";
            using var command = new NpgsqlCommand(sql, (NpgsqlConnection)ctx.Database.GetDbConnection());
            command.ExecuteNonQuery();
            logger?.Information("Tables {tableNames} have been successfully truncated !", tableNames);
        }


        return ctx;
    }

    /// <summary>
    /// Dispose the PostgreSQL container
    /// </summary>
    /// <returns></returns>
    public async Task DisposeAsync()
    {
        
        await _dbContainer.DisposeAsync();
    }
}

