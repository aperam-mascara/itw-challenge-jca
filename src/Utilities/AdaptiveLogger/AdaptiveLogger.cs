using Microsoft.Extensions.Logging;

namespace AdaptiveLogging;

/// <summary>
/// Logger adaptatif 
/// </summary>
public class AdaptiveLogger : ILogger
{
    private readonly string _categoryName;
    private readonly AdaptiveLoggerOptions _options;
    private readonly IExternalScopeProvider _scopeProvider;

    /// <summary>
    /// Sorties de log configurées.
    /// </summary>
    public LogOutput? Outputs;

    /// <summary>
    /// Constructeur du logger adaptatif.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <param name="options"></param>
    /// <param name="scopeProvider"></param>
    public AdaptiveLogger(string categoryName, AdaptiveLoggerOptions options, IExternalScopeProvider scopeProvider)
    {
        _categoryName = categoryName;
        _options = options;
        _scopeProvider = scopeProvider;
        if (_options.Outputs.Count == 0)
            _options.Outputs.Add(Console.WriteLine);

        _options.Outputs.ToList().ForEach(output =>
        {
            Outputs += output;
        });
    }

    /// <summary>
    /// Crée une portée de log.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
#pragma warning disable CS8633 // La nullabilité dans les contraintes pour le paramètre de type ne correspond pas aux contraintes pour le paramètre de type dans la méthode d'interface implémentée implicitement.
    public IDisposable BeginScope<TState>(TState? state) => _scopeProvider.Push(state);
#pragma warning restore CS8633 // La nullabilité dans les contraintes pour le paramètre de type ne correspond pas aux contraintes pour le paramètre de type dans la méthode d'interface implémentée implicitement.

    /// <summary>
    /// Vérifie si le niveau de log est activé selon les filtres configurés.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        // Vérifier si le log est activé selon les filtres
        return IsLogLevelEnabled(_categoryName, logLevel);
    }

    /// <summary>
    /// Vérifie si le niveau de log est activé pour une catégorie donnée.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    private bool IsLogLevelEnabled(string categoryName, LogLevel logLevel)
    {
        // Règle 1: Vérifier les correspondances exactes de catégorie
        if (_options.LogLevelFilters.TryGetValue(categoryName, out var level))
        {
            return logLevel >= level;
        }

        // Règle 2: Vérifier les correspondances de préfixe
        foreach (var filter in _options.LogLevelFilters)
        {
            string prefix = filter.Key;
            if (prefix.EndsWith('*') && categoryName.StartsWith(prefix[..^1]))
            {
                return logLevel >= filter.Value;
            }
        }

        // Règle 3: Utiliser la valeur par défaut
        if (_options.LogLevelFilters.TryGetValue("Default", out var defaultLevel))
        {
            return logLevel >= defaultLevel;
        }

        // Niveau par défaut si rien n'est configuré
        return logLevel >= LogLevel.Information;
    }

    /// <summary>
    /// Convertit le scope en chaîne de caractères.
    /// </summary>
    /// <param name="Level"></param>
    /// <param name="shortcut"></param>
    /// <returns></returns>
    private static string GetFormattedLogLevel(LogLevel Level,bool shortcut)
    {
        string _level = Level.ToString();

        return shortcut && _level.Length>=2 ? _level[..2] : _level;
    }

    /// <summary>
    /// Formate l'heure selon les options configurées.
    /// </summary>
    /// <param name="show"></param>
    /// <returns></returns>
    private static string GetFormattedTime( bool show)
    => show ? $"[{DateTime.Now:HH:mm:ss}]" : string.Empty;


    private static string GetCategoryName(string categoryName,bool show)
    => show ? $": [{categoryName}]" : string.Empty;
    /// <summary>
    /// Méthode de log principale.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <param name="formatter"></param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {

        if (!IsEnabled(logLevel))
            return;
        if (Outputs is null)
            return;
        var message = formatter(state, exception);
        var logPrefix =_options.ShowPrefix? $"{GetFormattedTime(_options.ShowTime)} {GetFormattedLogLevel(logLevel,_options.ShortcutPrefix)} {GetCategoryName(_categoryName,_options.ShowCategory)}":string.Empty;

        // Ajouter les scopes si nécessaire
        if (_options.IncludeScopes)
        {
            _scopeProvider.ForEachScope((scope, state) =>
            {
                logPrefix += $" => {scope}";
            }, state: null as object);
        }

        // Log dans la console du navigateur
        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
                Outputs($"{logPrefix} - {message}");
                break;
            case LogLevel.Information:
                Outputs($"{logPrefix} - {message}");
                break;
            case LogLevel.Warning:
                Outputs($"⚠️ {logPrefix} - {message}");
                break;
            case LogLevel.Error:
            case LogLevel.Critical:
                Outputs($"🔴 {logPrefix} - {message}");
                if (exception != null)
                    Outputs(exception.ToString());
                break;
            default:
                Outputs(message);
                break;
        }
    }
}
