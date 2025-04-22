namespace Chat.Client.Services.Impl;

/// <summary>
/// Implementation of the chat logger service.
/// </summary>
public class ChatLoggerService: IChatLoggerService
{

    /// <summary>
    /// Default category name for the logger.
    /// </summary>
    public const string DEFAULT_CATEGORY_NAME = "Chat.Client";

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatLoggerService"/> class.
    /// </summary>
    /// <param name="loggerProvider"></param>
    /// <param name="configuration"></param>
    public ChatLoggerService(ILoggerProvider loggerProvider, IConfiguration configuration)
    {
        string categoryName = configuration.GetValue<string>("LogChatCategoryName") ?? DEFAULT_CATEGORY_NAME;
        this.Logger = loggerProvider.CreateLogger(categoryName);
    }

    /// <summary>
    /// Gets the logger instance.
    /// </summary>
    public ILogger Logger { get; private init; }
}
