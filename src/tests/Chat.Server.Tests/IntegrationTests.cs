using Chat.Server.data;
using Chat.Shared.Dtos;
using Chat.Shared.Models;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
namespace Chat.Server.Tests
{
    /// <summary>
    /// Create Integration Tests
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="output"></param>
    public class IntegrationTests(WebAppFactory factory, ITestOutputHelper output) : WebClassFixture(factory, output)
    {
        /// <summary>
        /// Health Check of the server must be Ok
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HealthCheck_ShouldReturnHealthy()
        {
            
            // Act
            var response = await HttpClient.GetAsync("/health");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        /// <summary>
        /// Get Users should not be null but may be empty
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUsers_ShouldReturnUsersList()
        {
            // Act
            var response = await HttpClient.GetAsync("/chat/users");
            var users = await response.Content.ReadFromJsonAsync<List<User>>();

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            users.ShouldNotBeNull();
        }

        /// <summary>
        /// GetOrCreateUser should create a new user if it does not exist else return existing user
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetOrCreateUser_ShouldCreateNewUser()
        {
            // Arrange
            var username = $"testuser_{Guid.NewGuid()}";

            // Act
            var response = await HttpClient.GetAsync($"/chat/users/{username}");
            var user = await response.Content.ReadFromJsonAsync<User>();

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            user.ShouldNotBeNull();
            user.Username.ShouldBe(username);
        }

        /// <summary>
        /// SendMessage should create a new message in the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SendMessage_ShouldCreateNewMessage()
        {
            // Arrange
            var sender = await CreateUser("sender");
            var receiver = await CreateUser("receiver");

            var messageDto = new SendMessageDto
            {
                Content = "Test message",
                SenderId = sender.Id,
                ReceiverId = receiver.Id
            };

            // Act
            var response = await HttpClient.PostAsJsonAsync("/chat/messages", messageDto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var message = await response.Content.ReadFromJsonAsync<Message>();
            message.ShouldNotBeNull();
            message.Content.ShouldBe(messageDto.Content);

            
        }


        /// <summary>
        /// GetMessages should return messages between two users
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetMessages_ShouldReturnConversationMessages()
        {
            // Arrange
            var sender = await CreateUser("sender2");
            var receiver = await CreateUser("receiver2");

            var messageDto = new SendMessageDto
            {
                Content = "Test message",
                SenderId = sender.Id,
                ReceiverId = receiver.Id
            };

            await HttpClient.PostAsJsonAsync("/chat/messages", messageDto);

            // Act
            var response = await HttpClient.GetAsync($"/chat/messages/{sender.Id}/{receiver.Id}");
            var messages = await response.Content.ReadFromJsonAsync<List<Message>>();

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            messages.ShouldNotBeNull();
            messages.ShouldNotBeEmpty();
            messages.Count.ShouldBe(1);
            messages.ShouldAllBe(m => m.SenderId == sender.Id && m.ReceiverId == receiver.Id);
            messages.First().Content.ShouldBe(messageDto.Content);
        }

        private async Task<User> CreateUser(string username)
        {
            var response = await HttpClient.GetAsync($"/chat/users/{username}");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            User? user= await response.Content.ReadFromJsonAsync<User>();

            return user ?? throw new ApplicationException("User cannot be deserialized from the content of http request"); 
        }
    }
}
