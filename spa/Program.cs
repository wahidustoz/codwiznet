using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using spa.Services;

namespace spa
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddHttpClient("GithubClient",
                client =>
                {
                    client.BaseAddress = new Uri(builder.Configuration["codwiznet:Github:BaseUrl"].ToString());
                });

            builder.Services.AddHttpClient("MarkdownClient",
                client =>
                {
                    client.BaseAddress = new Uri(builder.Configuration["codwiznet:Github:MarkdownUrl"].ToString());
                });

            builder.Services.AddTransient<IGithubClient, GithubClient>();
            builder.Services.AddTransient<IMarkdownClient, MarkdownClient>();
            builder.Services.AddTransient<PostsRepo>();

            await builder.Build().RunAsync();
        }

    }
}
