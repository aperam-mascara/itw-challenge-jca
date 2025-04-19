using Chat.Shared.Dtos;
using Chat.Shared.Models;
using System.Net.Http.Json;

namespace Chat.Client.Services.Impl;

/// <summary>
/// Implementation of the message service.
/// </summary>
/// <remarks>
/// Constructor for MessageService.
/// </remarks>
/// <param name="httpClient"></param>
internal class MessageService(HttpClient httpClient) : IMessageService
{
    private readonly HttpClient _httpClient = httpClient;

    /// <summary>
    /// Gets the messages between two users.
    /// </summary>
    /// <param name="senderId"></param>
    /// <param name="receiverId"></param>
    /// <returns></returns>
    public async Task<List<Message>> GetMessagesAsync(int senderId, int receiverId)
    {
        try
        {
            var messages = await _httpClient.GetFromJsonAsync<List<Message>>($"/chat/messages/{senderId}/{receiverId}");
            return messages ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<bool> SendMessageAsync(SendMessageDto messageDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/chat/messages", messageDto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
