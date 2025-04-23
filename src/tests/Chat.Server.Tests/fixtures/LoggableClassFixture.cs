using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Xunit.Abstractions;

namespace Chat.Server.Tests.fixtures;

/// <summary>
/// Loggable Class Fixture for testing
/// </summary>
/// <typeparam name="T"></typeparam>
public class LoggableClassFixture<T> : IClassFixture<T> where T : class
{
    /// <summary>
    /// Configuration for testing
    /// </summary>
    protected IConfiguration Configuration { get; private init; }

    /// <summary>
    /// Logger support for testing
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="output"></param>
    public LoggableClassFixture(T factory, ITestOutputHelper output)
    {
        Factory = factory;
        Configuration = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", false, true)
                       .Build();
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .Enrich.FromLogContext()
            .WriteTo.TestOutput(
                output,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
            );
        Logger = loggerConfiguration.CreateLogger();
    }

    /// <summary>
    /// Factory for testing
    /// </summary>
    protected T Factory { get; }

    /// <summary>
    /// Logger for testing
    /// </summary>
    protected Logger Logger { get; init; }
}
