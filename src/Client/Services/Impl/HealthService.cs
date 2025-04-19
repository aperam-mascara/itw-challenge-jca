namespace Chat.Client.Services.Impl;

/// <summary>
/// Implementation of health check service.
/// </summary>
/// <remarks>
/// Constructor for HealthService.
/// </remarks>
/// <param name="httpClient"></param>
internal class HealthService(HttpClient httpClient) : IHealthService
{
    private readonly HttpClient _httpClient = httpClient;

    /// <summary>
    /// Checks if the server is healthy by sending a GET request to the /health endpoint.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsHealthyAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
