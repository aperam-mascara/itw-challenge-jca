using Chat.Shared.data;
using Chat.Shared.Dtos;
using Chat.Shared.Models;
using Microsoft.EntityFrameworkCore;
namespace Chat.Server;

/// <summary>
/// A set of extension methods for configuring chat routes.
/// </summary>
internal static class HostExtensions
{
    /// <summary>
    /// Configures the chat routes for the application.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="app"></param>
    /// <returns></returns>
    internal static WebApplication ConfigureChatRoutes<TDbContext>(this WebApplication app) where TDbContext : DbContext, IChatDbContext
    {
        

        var chat = app.MapGroup("chat").WithTags("chat");

        //Get All Users
        chat.MapGet("/users" , async (TDbContext dbContext) =>
        {
            var users = await dbContext.Users.ToListAsync();
            return Results.Ok(users);
        });


        //Get User by Id, add it if not exists 
        chat.MapGet("/users/{username}", async (string username, TDbContext dbContext) =>
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user is null)
            {
                // Create new user if not found
                user = new User { Username = username };
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                // Update last active time
                user.LastActive = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();
            }

            return Results.Ok(user);
        });

        //Get Messages send or received by a user and another user
        chat.MapGet("/messages/{senderId:int}/{receiverId:int}", async (int senderId, int receiverId, TDbContext dbContext) =>
        {
            var messages = await dbContext.Messages
                .Where(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return Results.Ok(messages);
        });

        //Send a message from one user to another
        chat.MapPost("/messages", async (SendMessageDto messageDto, TDbContext dbContext) =>
        {
            var sender = await dbContext.Users.FindAsync(messageDto.SenderId);
            var receiver = await dbContext.Users.FindAsync(messageDto.ReceiverId);

            if (sender is null || receiver is null)
            {
                return Results.BadRequest("Sender or receiver not found");
            }

            var message = new Message
            {
                Content = messageDto.Content,
                SenderId = messageDto.SenderId,
                SenderUsername = sender.Username,
                ReceiverId = messageDto.ReceiverId,
                ReceiverUsername = receiver.Username,
                Timestamp = DateTime.UtcNow
            };

            dbContext.Messages.Add(message);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/messages/{message.Id}", message);
        });
        return app;
    }
}
