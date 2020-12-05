using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace spa.Services
{
    public interface IMarkdownClient
    {
        Task<string> ParseMarkdown(string markdown, ParseMode mode = ParseMode.GithubFlavoredMarkdown);
        Task<string> ParseMarkdownFromUrl(string url, ParseMode mode = ParseMode.GithubFlavoredMarkdown);
    }

    public class MarkdownClient : IMarkdownClient
    {
        private readonly HttpClient mMarkdownClient;
        private readonly HttpClient mClient;
        private readonly ILogger<MarkdownClient> mLogger;

        public MarkdownClient(
            IHttpClientFactory clientFactory,
            ILogger<MarkdownClient> logger)
        {
            mMarkdownClient = clientFactory.CreateClient("MarkdownClient");
            mClient = clientFactory.CreateClient();
            mLogger = logger;
        }

        public async Task<string> ParseMarkdown(string markdown, ParseMode mode = ParseMode.GithubFlavoredMarkdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return string.Empty;
            }


            string htmlString = await RequestParse(markdown, mode);
            return htmlString;
        }

        public async Task<string> ParseMarkdownFromUrl(string url, ParseMode mode = ParseMode.GithubFlavoredMarkdown)
        {

            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri(url)
            };

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await mClient.SendAsync(requestMessage);
            }
            catch (Exception e)
            {
                mLogger.LogError($"Request to download Markdown file failed at {nameof(MarkdownClient)}, message: {e.Message}");
                return string.Empty;
            }

            if (response.IsSuccessStatusCode)
            {
                return await RequestParse(await response.Content.ReadAsStringAsync(), mode);
            }
            else
            {
                mLogger.LogError(response.RequestMessage + response.ReasonPhrase);
            }

            return string.Empty;
        }

        private async Task<string> RequestParse(string markdown, ParseMode mode)
        {
            string modeString = mode == ParseMode.GithubFlavoredMarkdown ? "gfm" : "markdown";

            JsonContent httpContent = JsonContent.Create(new MarkdownRequestBody
            {
                Text = markdown,
                Mode = modeString
            });

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await mMarkdownClient.PostAsync("", httpContent);
            }
            catch (Exception e)
            {
                mLogger.LogError($"Request to parse Markdown failed at {nameof(MarkdownClient)}, message: {e.Message}");
                return string.Empty;
            }

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                mLogger.LogError(response.RequestMessage + response.ReasonPhrase);
            }

            return string.Empty;
        }

    }

    public class MarkdownRequestBody
    {
        public string Text { get; set; }
        public string Mode { get; set; }
        public string Context { get; set; }
    }

    public enum ParseMode
    {
        Markdown,
        GithubFlavoredMarkdown
    }
}