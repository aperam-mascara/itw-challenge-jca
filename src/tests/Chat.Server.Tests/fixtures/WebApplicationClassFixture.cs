using Microsoft.AspNetCore.Mvc.Testing;
using Serilog;
using Xunit.Abstractions;

namespace Chat.Server.Tests.fixtures;

/// <summary>
/// Web Class Fixture for testing
/// </summary>
public class WebApplicationClassFixture<TWebAppFactory,TEntryPoint> : LoggableClassFixture<TWebAppFactory> 
    where TWebAppFactory : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    /// <summary>
    /// Web ApplicationFixture Factory for testing
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="output"></param>
    public WebApplicationClassFixture(TWebAppFactory factory, ITestOutputHelper output):base(factory, output)
    {


        HttpClient = factory
        .WithWebHostBuilder(builder =>
        {

            builder.ConfigureServices(services =>
            {

                services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.TestOutput(
                        output,
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
                    ));
            });
        })
        .CreateClient();

    }

    /// <summary>
    /// HttpClient for testing
    /// </summary>
    protected HttpClient HttpClient { get; init; }



}
