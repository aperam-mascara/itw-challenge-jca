using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using XUnitLogging;
using Xunit.Abstractions;
#pragma warning disable IDE0130 // Le namespace ne correspond pas à la structure de dossiers
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Le namespace ne correspond pas à la structure de dossiers

/// <summary>
/// XUnit Logging Extensions
/// </summary>
public static class DependencyServiceExtensions
{

    /// <summary>
    /// Add XUnit logging
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="outputFactory"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddXUnitLogging(this ILoggingBuilder builder, IConfiguration configuration, Func<ITestOutputHelper>? outputFactory=default, Action<XUnitLoggerConfiguration>? configure = default)
    {
        XUnitLoggerConfiguration _options = new();
        configure?.Invoke(_options);

        builder.AddConfiguration(configuration);
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, XUnitLoggerProvider>(serviceProvider =>
            {
                return new XUnitLoggerProvider(outputFactory?.Invoke() ?? default, _options);
            }));
        if(configure is not null)
            builder.Services.Configure(configure);
        return builder;
    }

    /// <summary>
    /// Add XUnit logging
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="outputHelperProvider"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddXUnitLogging(this ILoggingBuilder builder, IConfiguration configuration,ITestOutputHelperProvider outputHelperProvider, Action<XUnitLoggerConfiguration>? configure = default)
    {
        XUnitLoggerConfiguration _options = new();
        configure?.Invoke(_options);

        builder.AddConfiguration(configuration);
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, XUnitLoggerProvider>(serviceProvider =>
            {
                return new XUnitLoggerProvider(outputHelperProvider, _options);
            }));
        if (configure is not null)
            builder.Services.Configure(configure);
        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddXUnitLogging<T>(this ILoggingBuilder builder, IConfiguration configuration,  Action<XUnitLoggerConfiguration>? configure = default)
        where T:ITestOutputHelperProvider
    {
        XUnitLoggerConfiguration _options = new();
        configure?.Invoke(_options);

        builder.AddConfiguration(configuration);
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, XUnitLoggerProvider>(serviceProvider =>
            {
                var outputHelperProvider= serviceProvider.GetRequiredService<T>();
                return new XUnitLoggerProvider(outputHelperProvider, _options);
            }));
        if (configure is not null)
            builder.Services.Configure(configure);
        return builder;
    }

}
