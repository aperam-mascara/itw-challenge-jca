using Chat.Shared.Models;

namespace Chat.Client.Services;


/// <summary>
/// Interface for user service.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Event triggered when the current user changes.
    /// </summary>
    event Action<User?>? OnUserChanged;

    /// <summary>
    /// Gets the current user asynchronously.
    /// </summary>
    /// <returns>Current User</returns>
    Task<User?> GetCurrentUserAsync();


    /// <summary>
    /// Logs in a user asynchronously.
    /// </summary>
    /// <param name="username"></param>
    /// <returns>User logged in</returns>
    Task<User> LoginAsync(string username);

    /// <summary>
    /// Gets the stored username asynchronously.
    /// </summary>
    /// <returns>a list of users</returns>
    Task<List<User>> GetUsersAsync();

    /// <summary>
    /// Stores the username asynchronously.
    /// </summary>
    /// <returns></returns>
    Task LogoutAsync();
}
