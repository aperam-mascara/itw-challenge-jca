using Chat.Shared.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Chat.Shared.Models;
using Chat.Shared.Dtos;
using Microsoft.Extensions.Options;
namespace Chat.Routes;

/// <summary>
/// Options for configuring chat routes.
/// </summary>
internal sealed class ChatRouteOptions
{
    /// <summary>
    /// The default section name for chat routes.
    /// </summary>
    public const string SECTION_NAME = "Chat";

    /// <summary>
    /// The default path for chat routes.
    /// </summary>
    public string? BasePath { get; set; }


    /// <summary>
    /// The OpenApiTag .
    /// </summary>
    public string? OpenApiTag { get; set; }

    /// <summary>
    /// The path for user routes.
    /// </summary>

    public string? UserPath { get; set; }

    /// <summary>
    /// The path for message routes.
    /// </summary>

    public string? MessagePath { get; set; }
}


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
    /// <param name="options"></param>
    /// <returns></returns>
    public static IHost ConfigureChatRoutes<TDbContext>(this WebApplication app,Action<ChatRouteOptions>? options=default) where TDbContext : DbContext,IChatDbContext
    {
        var opt=app.Services.GetRequiredService<IOptions<ChatRouteOptions>>().Value;
        options?.Invoke(opt);

        var chatPath = opt.BasePath ?? "chat";
        var userPath = opt.UserPath ?? "users";
        var messagePath = opt.MessagePath ?? "messages";

        var chat = app.MapGroup(chatPath).WithTags(opt.OpenApiTag ?? ChatRouteOptions.SECTION_NAME);

        //Get All Users
        chat.MapGet("/"+userPath, async (TDbContext dbContext) =>
        {
            var users = await dbContext.Users.ToListAsync();
            return Results.Ok(users);
        });


        //Get User by Id, add it if not exists 
        chat.MapGet("/"+userPath+"/{username}", async (string username, TDbContext dbContext) =>
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
        chat.MapGet("/"+messagePath+"/{senderId:int}/{receiverId:int}", async (int senderId, int receiverId, TDbContext dbContext) =>
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
        chat.MapPost("/"+messagePath, async (SendMessageDto messageDto, TDbContext dbContext) =>
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

            return Results.Created($"/"+messagePath+"/{message.Id}", message);
        });
        return app;
    }
}
