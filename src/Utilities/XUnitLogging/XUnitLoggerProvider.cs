using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace XUnitLogging;

/// <summary>
/// XUnit Logger Provider
/// </summary>
[ProviderAlias("XUnit")]
internal sealed class XUnitLoggerProvider : ILoggerProvider
{
    
    private readonly ITestOutputHelper? _testOutputHelper;
    private readonly ITestOutputHelperProvider? _testOutputHelperProvider;
    private readonly XUnitLoggerConfiguration _options;
    private readonly LoggerExternalScopeProvider _scopeProvider = new ();
    private bool disposedValue;

    internal XUnitLoggerProvider(ITestOutputHelper? output, XUnitLoggerConfiguration options)
    {
        _testOutputHelper=output;
        _options=options;
    }

    internal XUnitLoggerProvider(Func<ITestOutputHelper>? outputBuilder, XUnitLoggerConfiguration options)
    {
        _testOutputHelper = outputBuilder?.Invoke() ?? default;
        _options = options;
    }
    internal XUnitLoggerProvider(ITestOutputHelperProvider? outputProvider, XUnitLoggerConfiguration options)
    {
        _testOutputHelperProvider = outputProvider;
        _options = options;
    }

    /// <summary>
    /// Create a logger instance with the specified category name.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public ILogger CreateLogger(string categoryName)
    {
        //if (_testOutputHelper is null)
        //    throw new ApplicationException("You must set ITestOutputHelper before  logging");
        XUnitLogger logger = new (_testOutputHelper, _scopeProvider, categoryName, _options);
        logger.SetTestOutputHelperProvider(_testOutputHelperProvider);
        return logger;
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: supprimer l'état managé (objets managés)
            }

            // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
            // TODO: affecter aux grands champs une valeur null
            disposedValue = true;
        }
    }

    // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
    // ~XUnitLoggerProvider()
    // {
    //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
