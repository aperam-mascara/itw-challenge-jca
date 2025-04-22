namespace Chat.Shared.Models;

/// <summary>
/// This class represents a user in the chat application.
/// </summary>
public record User
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The username of the user.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// The timestamp when the user was last active.
    /// </summary>
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
}