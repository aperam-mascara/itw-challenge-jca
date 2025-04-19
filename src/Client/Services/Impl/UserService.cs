using Chat.Shared.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Chat.Client.Services.Impl;

/// <summary>
/// Implementation of the user service.
/// </summary>
/// <remarks>
/// Constructor for UserService.
/// </remarks>
/// <param name="httpClient"></param>
/// <param name="jSRuntime"></param>
internal class UserService(HttpClient httpClient, IJSRuntime jSRuntime) : IUserService
{
    private readonly HttpClient _httpClient = httpClient;
    private User? _currentUser;
    private const string CurrentUserKey = "currentUser";
    private readonly IJSRuntime JS = jSRuntime;

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
        if (_currentUser != null)
            return _currentUser;

        var username = await GetStoredUsernameAsync();

        if (string.IsNullOrEmpty(username))
            return null;

        return await LoginAsync(username);
    }


    /// <summary>
    /// Logs in a user asynchronously.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<User> LoginAsync(string username)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/chat/users/{username}");
            response.EnsureSuccessStatusCode();

            _currentUser = await response.Content.ReadFromJsonAsync<User>();

            if (_currentUser != null)
            {
                await StoreUsernameAsync(_currentUser.Username);
                OnUserChanged?.Invoke(_currentUser);
            }

            return _currentUser!;
        }
        catch (Exception)
        {
            throw new Exception($"Failed to login as {username}");
        }
    }

    /// <summary>
    /// Gets all users asynchronously.
    /// </summary>
    /// <returns></returns>
    public async Task<List<User>> GetUsersAsync()
    {
        try
        {
            var users = await _httpClient.GetFromJsonAsync<List<User>>("/chat/users") ?? [];
            return users;
        }
        catch (Exception)
        {
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
