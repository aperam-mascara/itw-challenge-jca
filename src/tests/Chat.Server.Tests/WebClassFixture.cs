using Xunit.Abstractions;

namespace Chat.Server.Tests;

/// <summary>
/// Web Class Fixture for testing
/// </summary>
public class WebClassFixture : IClassFixture<WebAppFactory>
{
    /// <summary>
    /// Web ApplicationFixture Factory for testing
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="output"></param>
    public WebClassFixture(WebAppFactory factory, ITestOutputHelper output)
    {
        this.Factory = factory;
        SetOutputHelper(output);
        this.HttpClient = factory.CreateClient();
    }

    


    /// <summary>
    /// Get the Web Application Factory
    /// </summary>
    public WebAppFactory Factory { get; }

    /// <summary>
    /// Test Output Helper
    /// </summary>
    public ITestOutputHelper Output => Factory.OutputHelper;

    /// <summary>
    /// HttpClient for testing
    /// </summary>
    protected HttpClient HttpClient { get; init; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="output"></param>
    public void SetOutputHelper(ITestOutputHelper output)
    {
        Factory.OutputHelper = output;
    }
}
