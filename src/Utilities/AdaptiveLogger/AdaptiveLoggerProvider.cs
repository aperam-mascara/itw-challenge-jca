using Microsoft.Extensions.Logging;

namespace AdaptiveLogging;
/// <summary>
/// Délégué pour la sortie de log.
/// </summary>
/// <param name="message"></param>
public delegate void LogOutput(string message);

/// <summary>
/// Provider pour le logger adaptatif.
/// </summary>
/// <param name="options"></param>
public class AdaptiveLoggerProvider(AdaptiveLoggerOptions options) : ILoggerProvider
{
    private readonly AdaptiveLoggerOptions _options = options;
    private readonly IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();
    private bool disposedValue;

    /// <summary>
    /// Crée un logger pour une catégorie donnée.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new AdaptiveLogger(categoryName, _options, _scopeProvider);
    }

    /// <summary>
    /// Libère les ressources utilisées par le logger provider.
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
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
    // ~BrowserConsoleLoggerProvider()
    // {
    //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
    //     Dispose(disposing: false);
    // }

    /// <summary>
    /// Libère les ressources utilisées par le logger provider.
    /// </summary>
    public void Dispose()
    {
        // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
