using Chat.Shared.Dtos;
using Chat.Shared.Models;

namespace Chat.Client.Services;

/// <summary>
/// Interface for message service.
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Gets the messages between two users.
    /// </summary>
    /// <param name="senderId"></param>
    /// <param name="receiverId"></param>
    /// <returns></returns>
    Task<List<Message>> GetMessagesAsync(int senderId, int receiverId);

    /// <summary>
    /// Sends a message from one user to another.
    /// </summary>
    /// <param name="messageDto"></param>
    /// <returns></returns>
    Task<bool> SendMessageAsync(SendMessageDto messageDto);
}