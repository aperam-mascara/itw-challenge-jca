/*namespace Chat.Client;


/// <summary>
/// Options pour le logger adaptatif.
/// </summary>
public class AdaptiveLoggerOptions
{
    private readonly HashSet<LogOutput> _outputs = [];

    /// <summary>
    /// Indique si les scopes doivent être inclus dans les messages de log.
    /// </summary>
    public bool IncludeScopes { get; set; } = true;

    /// <summary>
    /// Filtres de niveau de log pour les catégories spécifiques.
    /// </summary>
    public IDictionary<string, LogLevel> LogLevelFilters { get; set; } = new Dictionary<string, LogLevel>();

    /// <summary>
    /// Collection des sorties de log.
    /// </summary>
    public HashSet<LogOutput> Outputs => _outputs;

    /// <summary>
    /// Ajoute une sortie de log pour la console du navigateur.
    /// </summary>
    /// <returns></returns>
    public AdaptiveLoggerOptions AddConsoleOutput()
    => AddOutput(Console.WriteLine);

    /// <summary>
    /// Ajoute une sortie de log personnalisée.
    /// </summary>
    /// <param name="output"></param>
    /// <returns></returns>
    public AdaptiveLoggerOptions AddOutput(LogOutput output)
    {
        _outputs.Add(output);
        return this;
    }
}
/// <summary>
/// Délégué pour la sortie de log.
/// </summary>
/// <param name="message"></param>
public delegate void LogOutput(string message);

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
        if (_options.Outputs.Count==0)
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
        var logPrefix = $"[{DateTime.Now:HH:mm:ss}] {logLevel}: {_categoryName}";

        // Ajouter les scopes si nécessaire
        if (_options.IncludeScopes)
        {
            _scopeProvider.ForEachScope((scope, state) =>
            {
                logPrefix += $" => {scope}";
            }, state:null as object);
        }

        // Log dans la console du navigateur
        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
                Outputs($"{logPrefix} - {message}");
                break;
            case LogLevel.Information:
                Outputs ($"{logPrefix} - {message}");
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

/// <summary>
/// Extensions pour le logger adaptatif.
/// </summary>
public static class AdaptiveLoggerExtensions
{
    /// <summary>
    /// Ajoute le logger adaptatif à la configuration de logging.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddBrowserConsole(this ILoggingBuilder builder)
    {
        return builder.AddBrowserConsole(options => { });
    }

    /// <summary>
    /// Ajoute le logger adaptatif à la configuration de logging avec une configuration personnalisée.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddBrowserConsole(this ILoggingBuilder builder, Action<AdaptiveLoggerOptions> configure)
    {
        var options = new AdaptiveLoggerOptions();
        configure(options);

        // Appliquer la configuration du fichier appsettings.json si disponible
        builder.Services.AddSingleton<AdaptiveLoggerOptions>(services =>
        {
            var config = services.GetService<IConfiguration>();

            if (config != null)
            {
                var loggingSection = config.GetSection("Logging");
                var logLevelSection = loggingSection.GetSection("LogLevel");

                foreach (var kvp in logLevelSection.GetChildren())
                {
                    if (Enum.TryParse<LogLevel>(kvp.Value, out var level))
                    {
                        options.LogLevelFilters[kvp.Key] = level;
                    }
                }
            }

            // Appliquer la configuration programmatique ensuite
            configure(options);
            return options;
        });

        builder.Services.AddSingleton<ILoggerProvider, AdaptiveLoggerProvider>();
        return builder;
    }
}

*/