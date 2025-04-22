using AdaptiveLogging;
using Chat.Client;
using Chat.Client.Services;
using Chat.Client.Services.Impl;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_URL"] ?? "https://localhost:8044/") });

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddAdaptiveLogger(cfg =>
{
    cfg.ShortcutPrefix = true;
    cfg.ShowCategory = false;
    cfg.ShowTime = false;
});

builder.Services.AddMudServices();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IHealthService, HealthService>();
builder.Services.AddScoped<IChatLoggerService, ChatLoggerService>();

await builder.Build().RunAsync();
