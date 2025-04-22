using Xunit.Abstractions;

namespace XUnitLogging;

/// <summary>
/// Output helper provider interface.
/// </summary>
public interface ITestOutputHelperProvider
{
    /// <summary>
    /// Return a TestOutputHelper
    /// </summary>
    ITestOutputHelper? TestOutputHelper();
}
