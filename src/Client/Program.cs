using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Chat.Client;
using Chat.Client.Services;
using Chat.Client.Services.Impl;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_URL"] ?? "https://localhost:5001") });

builder.Services.AddMudServices();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IHealthService, HealthService>();

await builder.Build().RunAsync();
