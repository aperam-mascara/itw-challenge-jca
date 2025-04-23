namespace Chat.Shared.Dtos;

/// <summary>
///  Send message DTO. its used to send message from one user to another.
/// </summary>
public record SendMessageDto
{
    /// <summary>
    /// The content of the message.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// The ID of the sender.
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// The ID of the receiver.
    /// </summary>
    public int ReceiverId { get; set; }
}
