namespace Chat.Client.Services;

/// <summary>
/// Interface for the chat logger service.
/// </summary>
public interface IChatLoggerService
{
    /// <summary>
    /// Gets the logger instance.
    /// </summary>
    ILogger Logger { get; }
}
