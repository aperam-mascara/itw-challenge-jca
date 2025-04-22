using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AdaptiveLogging;

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
    public static ILoggingBuilder AddAdaptiveLogger(this ILoggingBuilder builder)
    {
        return builder.AddAdaptiveLogger(options => { });
    }

    /// <summary>
    /// Ajoute le logger adaptatif à la configuration de logging avec une configuration personnalisée.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddAdaptiveLogger(this ILoggingBuilder builder, Action<AdaptiveLoggerOptions> configure)
    {
        var options = new AdaptiveLoggerOptions();
        configure(options);

        // Appliquer la configuration du fichier appsettings.json si disponible
        builder.Services.AddSingleton(services =>
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