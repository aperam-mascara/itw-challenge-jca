namespace Chat.Shared.models;

public record User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
}