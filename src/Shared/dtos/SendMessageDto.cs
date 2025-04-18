namespace Chat.Shared.dtos;

public record SendMessageDto
{
    public required string Content { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
}
