namespace Chat.Shared.models;

public record Message
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public int SenderId { get; set; }
    public required string SenderUsername { get; set; }

    public int ReceiverId { get; set; }
    public required string ReceiverUsername { get; set; }
}
