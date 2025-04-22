using Microsoft.Extensions.Logging;

namespace AdaptiveLogging;

/// <summary>
/// Options pour le logger adaptatif.
/// </summary>
public class AdaptiveLoggerOptions
{
    private readonly HashSet<LogOutput> _outputs = [];

    /// <summary>
    /// 
    /// </summary>
    public bool ShowPrefix { get; set; } = true;

    /// <summary>
    /// Indique si le logger couper le scopes de log.
    /// </summary>
    public bool ShortcutPrefix { get; set; }

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
    /// Indique si le logger doit afficher la date et l'heure dans les messages de log.
    /// </summary>
    public bool ShowTime { get; set; } = true;

    /// <summary>
    /// Indique si le logger doit afficher le nom de la catégorie dans les messages de log.
    /// </summary>
    public bool ShowCategory { get; set; } = true;

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
