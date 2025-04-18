using Chat.Shared.models;
using Microsoft.EntityFrameworkCore;

namespace Chat.Server.data;


/// <summary>
/// This class represents the database context for the chat application.
/// </summary>
public class ChatDbContext:DbContext
{
    /// <summary>
    /// Constructor for ChatDbContext.
    /// </summary>
    /// <param name="options"></param>
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
    }

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
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Configure Message entity
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.SenderUsername).IsRequired();
            entity.Property(e => e.ReceiverUsername).IsRequired();
        });
    }
}
