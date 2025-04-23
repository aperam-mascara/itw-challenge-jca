using Chat.Server.Data;
using Chat.Server.Tests.fixtures;
using Chat.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Collections.Immutable;
using Xunit.Abstractions;
using XUnitPostgreSQL;
namespace Chat.Server.Tests;



/// <summary>
/// Tests for ChatDbContext.
/// </summary>
/// <remarks>
/// Constructor for ChatDbContextTests.
/// </remarks>
/// <param name="fixture"></param>
/// <param name="output"></param>
public class ChatDbContextTests(PostgresContainerFixture<ChatDbContext> fixture, ITestOutputHelper output) : LoggableClassFixture<PostgresContainerFixture<ChatDbContext>>(fixture, output)
{
    private readonly PostgresContainerFixture<ChatDbContext> fixture = fixture;

    private readonly IImmutableList<string> TableNames = [ChatDbContext.UserTableName, ChatDbContext.MessageTableName];


    /// <summary>
    /// Test to check if the database is created successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateUser_ShouldAddUserToDatabase()
    {
        //Cleanup
        using var context = await fixture.ContextAndResetTablesAsync(TableNames,Logger);

        // Arrange
        var username = "testuser";


        // Act
        var user = new User { Username = username };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Assert
        var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
        savedUser.ShouldNotBeNull();
        savedUser.Username.ShouldBe(username);
    }


    /// <summary>
    /// Test to check if a message can be created and saved in the database.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateMessage_ShouldAddMessageToDatabase()
    {
        //Cleanup
        using var context = await fixture.ContextAndResetTablesAsync(TableNames,Logger);


        // Arrange

        var sender = new User { Username = "sender" };
        var receiver = new User { Username = "receiver" };
        context.Users.AddRange(sender, receiver);
        await context.SaveChangesAsync();

        // Act
        var message = new Message
        {
            Content = "Test message",
            SenderId = sender.Id,
            SenderUsername = sender.Username,
            ReceiverId = receiver.Id,
            ReceiverUsername = receiver.Username
        };

        context.Messages.Add(message);
        await context.SaveChangesAsync();

        // Assert
        var savedMessage = await context.Messages.FirstOrDefaultAsync(m => m.Id == message.Id);
        savedMessage.ShouldNotBeNull();
        savedMessage.Content.ShouldBe("Test message");
        savedMessage.SenderId.ShouldBe(sender.Id);
        savedMessage.ReceiverId.ShouldBe(receiver.Id);
    }

    /// <summary>
    /// Test to check if messages can be retrieved for a specific user.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetUserMessages_ShouldReturnMessagesForUsers()
    {
        //Cleanup
        using var context = await fixture.ContextAndResetTablesAsync(TableNames,Logger);

        // Arrange

        var sender = new User { Username = "sender" };
        var receiver = new User { Username = "receiver" };
        context.Users.AddRange(sender, receiver);
        await context.SaveChangesAsync();

        var messages = new[]
        {
        new Message
        {
            Content = "Message 1",
            SenderId = sender.Id,
            SenderUsername = sender.Username,
            ReceiverId = receiver.Id,
            ReceiverUsername = receiver.Username
        },
        new Message
        {
            Content = "Message 2",
            SenderId = receiver.Id,
            SenderUsername = receiver.Username,
            ReceiverId = sender.Id,
            ReceiverUsername = sender.Username
        }
    };

        context.Messages.AddRange(messages);
        await context.SaveChangesAsync();

        // Act
        var conversationMessages = await context.Messages
            .Where(m =>
                (m.SenderId == sender.Id && m.ReceiverId == receiver.Id) ||
                (m.SenderId == receiver.Id && m.ReceiverId == sender.Id))
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        // Assert 
        conversationMessages.ShouldNotBeNull();
        conversationMessages.ShouldNotBeEmpty();
        conversationMessages.Count.ShouldBe(2);
        conversationMessages.ShouldAllBe(m => m.SenderId == sender.Id || m.SenderId == receiver.Id);
        conversationMessages[0].Content.ShouldBe("Message 1");
        conversationMessages[1].Content.ShouldBe("Message 2");
    }


    /// <summary>
    /// Test to check if a user can be created with a unique username.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UsernameShouldBeUnique()
    {
        //Cleanup
        using var context = await fixture.ContextAndResetTablesAsync(TableNames, Logger);

        // Arrange

        var username = "uniqueuser";
        var user1 = new User { Username = username };

        // Act
        context.Users.Add(user1);
        await context.SaveChangesAsync();

        var user2 = new User { Username = username };
        context.Users.Add(user2);


        // Assert that adding a duplicate username throws an exception
        Action act = () => context.SaveChanges();
        act.ShouldThrow<DbUpdateException>();
    }
}
