using Chat.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat.Shared.data;

/// <summary>
/// This interface defines the contract for the chat database context.
/// </summary>
public interface IChatDbContext
{
    /// <summary>
    /// DbSet for User entity.
    /// </summary>
    DbSet<User> Users { get; set; }

    /// <summary>
    /// DbSet for Message entity.
    /// </summary>
    DbSet<Message> Messages { get; set; } 
}
