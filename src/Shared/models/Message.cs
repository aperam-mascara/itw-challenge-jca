namespace Chat.Shared.Models;

/// <summary>
/// This class represents a message in the chat application.
/// </summary>
public record Message
{
    /// <summary>
    /// The unique identifier for the message.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The content of the message.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// The timestamp when the message was sent.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The ID of the sender.
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// The username of the sender.
    /// </summary>
    public required string SenderUsername { get; set; }

    /// <summary>
    /// The ID of the receiver.
    /// </summary>
    public int ReceiverId { get; set; }

    /// <summary>
    /// The username of the receiver.
    /// </summary>
    public required string ReceiverUsername { get; set; }
}
