using Chat.Shared.Dtos;
using Chat.Shared.Models;
using System.Net.Http.Json;

namespace Chat.Client.Services.Impl;

/// <summary>
/// Implementation of the message service.
/// </summary>
/// <param name="httpClient"></param>
/// <param name="logger"></param>
internal class MessageService(HttpClient httpClient,ILogger<MessageService> logger) : IMessageService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<MessageService> Logger = logger;
    /// <summary>
    /// Gets the messages between two users.
    /// </summary>
    /// <param name="senderId"></param>
    /// <param name="receiverId"></param>
    /// <returns></returns>
    public async Task<List<Message>> GetMessagesAsync(int senderId, int receiverId)
    {
        Logger.LogTrace("Try to get message between user:{senderId} and user:{receiverId}",senderId,receiverId);
        try
        {
            var messages = await _httpClient.GetFromJsonAsync<List<Message>>($"/chat/messages/{senderId}/{receiverId}");
            Logger.LogTrace("All messages are retrieved");
            return messages ?? [];
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Error while retrieving messages \n\t\t{ex.Message}",ex.Message);
            return [];
        }
    }

    public async Task<bool> SendMessageAsync(SendMessageDto message)
    {
        Logger.LogTrace($"{message.SenderId} try to send a message to {message.ReceiverId}",message.SenderId,message.ReceiverId);
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/chat/messages", message);
            Logger.LogTrace("Message {message.Content} was sended to {message.ReceiverId}", message.Content, message.ReceiverId);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Logger.LogWarning($"Error while sending messages \n\t\t{ex.Message}");
            return false;
        }
    }
}
