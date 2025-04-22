using Chat.Shared.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Chat.Client.Services.Impl;

/// <summary>
/// Implementation of the user service.
/// </summary>
/// <param name="httpClient"></param>
/// <param name="loggerProvider"></param>
/// <param name="configuration"></param>
/// <param name="jSRuntime"></param>
internal class UserService(HttpClient httpClient,ILoggerProvider loggerProvider,IConfiguration configuration, IJSRuntime jSRuntime) : IUserService
{
    private readonly HttpClient _httpClient = httpClient;
    private User? _currentUser;
    private const string CurrentUserKey = "currentUser";
    private readonly IJSRuntime JS = jSRuntime;
    private readonly ILogger Logger=loggerProvider.CreateLogger("Chat.Client");
    private readonly bool _autoLogin = configuration?.GetValue<bool>("AutoLogin") ??false;

    /// <summary>
    /// Authorize Auto Login
    /// </summary>
    public bool AuthorizeAutoLogin => _autoLogin;
    /// <summary>
    /// Event triggered when the current user changes.
    /// </summary>
    public event Action<User?>? OnUserChanged;

    /// <summary>
    /// Gets the current user asynchronously.
    /// </summary>
    /// <returns></returns>
    public async Task<User?> GetCurrentUserAsync()
    {
        Logger.LogTrace("Try get Current user");
        if (_currentUser != null)
        {
            Logger.LogTrace("The current user is {_currentUser.Username}", _currentUser.Username);
            return _currentUser;
        }

        Logger.LogTrace("Try get user from local storage");
        var username = await GetStoredUsernameAsync();

        if (string.IsNullOrEmpty(username))
        {
            Logger.LogTrace("No user ins local storage. Abort...");
            return null;
        }

        await LoginAsync(username);
        return _currentUser;
    }


    /// <summary>
    /// Logs in a user asynchronously.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> LoginAsync(string username)
    {
        Logger.LogTrace("Try {username} logged in",username);
        try
        {
            var response = await _httpClient.GetAsync($"/chat/users/{username}");
            response.EnsureSuccessStatusCode();

            _currentUser = await response.Content.ReadFromJsonAsync<User>();
           
            if (_currentUser != null)
            {
                await StoreUsernameAsync(_currentUser.Username);
                OnUserChanged?.Invoke(_currentUser);
                Logger.LogTrace("{username} is logged on { }",username, DateTime.UtcNow.ToString());
                return true;
            }
            Logger.LogTrace("{username} cannot log in", username);
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Error while logged in \n\t\t{ex.Message}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Gets all users asynchronously.
    /// </summary>
    /// <returns></returns>
    public async Task<List<User>> GetUsersAsync()
    {
        Logger.LogTrace("Try get all registered users");
        try
        {
            var users = await _httpClient.GetFromJsonAsync<List<User>>("/chat/users") ?? [];
            Logger.LogTrace("All users are retrieved");
            return users;
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Error while getting all users  \n\t\t{ex.Message}", ex.Message);
            return [];
        }
    }


    /// <summary>
    /// Retrieves the stored username from local storage asynchronously.
    /// </summary>
    /// <returns></returns>
    private async Task<string> GetStoredUsernameAsync()
    {
        try
        {
            return await (JS.InvokeAsync<string>("localStorage.getItem", CurrentUserKey)) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Stores the username in local storage asynchronously.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    private async Task StoreUsernameAsync(string username)
    {
        try
        {
            await (JS.InvokeVoidAsync("localStorage.setItem", CurrentUserKey, username));
        }
        catch
        {
            throw;
            // Ignore any errors when storing the username
        }
    }

    /// <summary>
    /// Logs out the current user asynchronously.
    /// </summary>
    /// <returns></returns>
    public async Task LogoutAsync()
    {
        _currentUser = null;
        try
        {
            await (JS.InvokeVoidAsync("localStorage.removeItem", CurrentUserKey));
        }
        catch
        {
            // Ignore any errors when removing the username
            
        }
        OnUserChanged?.Invoke(_currentUser);
    }
}
