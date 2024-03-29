using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorApp.Client.Utils;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AzureStaticWebApps.Blazor.Authentication;
using Blazored.Modal;


namespace BlazorApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var baseAddress = builder.Configuration["BaseAddress"] ?? builder.HostEnvironment.BaseAddress;
            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseAddress) });
            builder.Services.AddScoped<BackendApiRepository>();
            builder.Services.AddScoped<ClipboardService>();
            builder.Services.AddSingleton<AppState>();
            builder.Services.AddScoped<ClubCheck>();
            builder.Services.AddBlazoredModal();
            builder.Services.AddStaticWebAppsAuthentication();

            await builder.Build().RunAsync();
        }
    }
}