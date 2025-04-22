using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace XUnitLogging;
/// <summary>
/// XUnit Logger Configuration
/// </summary>
public sealed class XUnitLoggerConfiguration
{
    /// <summary>
    /// Gets or sets the minimum log level. Defaults to <see cref="LogLevel.Trace" />.
    /// </summary>
    public LogLevel  LogLevel{ get; set; }=LogLevel.Trace;
    /// <summary>
    /// Includes scopes when <see langword="true" />.
    /// </summary>
    public bool IncludeScopes { get; set; } = true;

    /// <summary>
    /// Includes category when <see langword="true" />.
    /// </summary>
    public bool IncludeCategory { get; set; }=true;

    /// <summary>
    /// Includes log level when <see langword="true" />.
    /// </summary>
    public bool IncludeLogLevel { get; set; } = true;

    /// <summary>
    /// Gets or sets format string used to format timestamp in logging messages. Defaults to <see langword="null" />.
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
    public string? TimestampFormat { get; set; }

    /// <summary>
    /// Gets or sets indication whether or not UTC timezone should be used to format timestamps in logging messages. Defaults to <see langword="false" />.
    /// </summary>
    public bool UseUtcTimestamp { get; set; } = false;
}
