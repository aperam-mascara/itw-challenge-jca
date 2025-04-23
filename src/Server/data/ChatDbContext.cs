using Chat.Shared.data;
using Chat.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat.Server.Data;


/// <summary>
/// This class represents the database context for the chat application.
/// </summary>
/// <remarks>
/// Constructor for ChatDbContext.
/// </remarks>
/// <param name="options"></param>
public class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options), IChatDbContext
{
    internal const string UserTableName = nameof(Users);

    internal const string MessageTableName = nameof(Messages);

    /// <summary>
    /// DbSet for User entity.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// DbSet for Message entity.
    /// </summary>
    public DbSet<Message> Messages { get; set; } = null!;


    /// <summary>
    /// Configures the model for the database context.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable(UserTableName);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Configure Message entity
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable(MessageTableName);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.SenderUsername).IsRequired();
            entity.Property(e => e.ReceiverUsername).IsRequired();
        });
    }
        
}
