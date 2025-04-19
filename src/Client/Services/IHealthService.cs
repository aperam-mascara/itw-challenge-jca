namespace Chat.Client.Services;

/// <summary>
/// Interface for health check service.
/// </summary>
public interface IHealthService
{
    /// <summary>
    /// Checks if the service is healthy.
    /// </summary>
    /// <returns></returns>
    Task<bool> IsHealthyAsync();
}
