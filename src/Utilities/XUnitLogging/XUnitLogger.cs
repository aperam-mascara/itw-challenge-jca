using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit.Abstractions;

namespace XUnitLogging;

/// <summary>
/// Logger for XUnit Integration
/// </summary>
public class XUnitLogger : ILogger
{
    private  ITestOutputHelper? _testOutputHelper;
    private ITestOutputHelperProvider? _testOutputHelperProvider;
    private readonly string _categoryName;
    private readonly XUnitLoggerConfiguration _options;
    private readonly LoggerExternalScopeProvider _scopeProvider;
    private const string DEFAULT_CATEGORY_NAME = "Unknown";

    /// <summary>
    /// Create XUnit Logger
    /// </summary>
    /// <param name="testOutputHelper"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static ILogger CreateLogger(ITestOutputHelper testOutputHelper, Action<XUnitLoggerConfiguration>? options)
    {
        XUnitLoggerConfiguration _options = new();
        options?.Invoke(_options);
        return new XUnitLogger(testOutputHelper, new LoggerExternalScopeProvider(), string.Empty,_options);
    }
    /// <summary>
    /// Create XUnit Logger
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static ILogger CreateLogger( Action<XUnitLoggerConfiguration>? options)
    {
        XUnitLoggerConfiguration _options = new();
        options?.Invoke(_options);
        return new XUnitLogger(null, new LoggerExternalScopeProvider(), string.Empty, _options);
    }
    /// <summary>
    /// Create XUnit Logger
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="testOutputHelper"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static ILogger<T> CreateLogger<T>(ITestOutputHelper testOutputHelper, Action<XUnitLoggerConfiguration>? options)
        where T : notnull
    {
        XUnitLoggerConfiguration _options = new();
        options?.Invoke(_options);
        return new XUnitLogger<T>(testOutputHelper, new LoggerExternalScopeProvider(), _options);
    }

    /// <summary>
    /// Create XUnit Logger
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="options"></param>
    /// <returns></returns>
    public static ILogger<T> CreateLogger<T>( Action<XUnitLoggerConfiguration>? options)
        where T : notnull
    {
        XUnitLoggerConfiguration _options = new();
        options?.Invoke(_options);
        return new XUnitLogger<T>(default, new LoggerExternalScopeProvider(), _options);
    }
    /// <summary>
    /// Create XUnit Logger
    /// </summary>
    /// <param name="testOutputHelper"></param>
    /// <param name="scopeProvider"></param>
    /// <param name="categoryName"></param>
    /// <param name="options"></param>
    internal XUnitLogger(ITestOutputHelper? testOutputHelper, LoggerExternalScopeProvider scopeProvider, string? categoryName, XUnitLoggerConfiguration options)
    {
        _testOutputHelper = testOutputHelper;
        _scopeProvider = scopeProvider;
        _categoryName = categoryName ?? DEFAULT_CATEGORY_NAME;
        _options= options;
    }

    /// <summary>
    /// Is log enabled according level
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel logLevel) => logLevel <=_options.LogLevel;


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    public IDisposable BeginScope<TState>([NotNull] TState state)
        where TState : notnull
        => _scopeProvider.Push(state);

    /// <summary>
    /// log
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <param name="formatter"></param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _testOutputHelper ??= _testOutputHelperProvider?.TestOutputHelper();
        if (_testOutputHelper is null)
        {
            return;
        }
        var sb = new StringBuilder();

        if (_options.TimestampFormat is not null)
        {
            var now = _options.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
            var timestamp = now.ToString(_options.TimestampFormat);
            sb.Append(timestamp).Append(' ');
        }

        if (_options.IncludeLogLevel)
        {
            sb.Append(GetLogLevelString(logLevel)).Append(' ');
        }

        if (_options.IncludeCategory)
        {
            sb.Append('[').Append(_categoryName).Append("] ");
        }

        sb.Append(formatter(state, exception));

        if (exception is not null)
        {
            sb.Append('\n').Append(exception);
        }

        // Append scopes
        if (_options.IncludeScopes)
        {
            _scopeProvider.ForEachScope((scope, state) =>
            {
                state.Append("\n => ");
                state.Append(scope);
            }, sb);
        }

        try
        {
            _testOutputHelper.WriteLine(sb.ToString());
        }
        catch
        {
            // This can happen when the test is not active
        }
    }

    /// <summary>
    /// Set the test output helper
    /// </summary>
    /// <param name="provider"></param>
    public void SetTestOutputHelperProvider(ITestOutputHelperProvider? provider)
    {
        _testOutputHelperProvider = provider;
    }

    private static string GetLogLevelString(LogLevel logLevel)
    => logLevel switch
    {
        LogLevel.Trace => "trce",
        LogLevel.Debug => "dbug",
        LogLevel.Information => "info",
        LogLevel.Warning => "warn",
        LogLevel.Error => "fail",
        LogLevel.Critical => "crit",
        _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
    };
}


/// <summary>
/// XUnit Logger for T
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class XUnitLogger<T> : XUnitLogger, ILogger<T>
    where T : notnull
{
    /// <summary>
    /// Create XUnit Logger
    /// </summary>
    /// <param name="testOutputHelper"></param>
    /// <param name="scopeProvider"></param>
    /// <param name="options"></param>
    internal XUnitLogger(ITestOutputHelper? testOutputHelper, LoggerExternalScopeProvider scopeProvider, XUnitLoggerConfiguration options)
        : base(testOutputHelper, scopeProvider, typeof(T).FullName, options)
    {
    }
}