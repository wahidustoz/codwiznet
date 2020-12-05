using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace spa.Services
{
    public interface IGithubClient
    {
        Task<string> GetPosts();
    }

    public class GithubClient : IGithubClient
    {
        private readonly IConfiguration mConfiguration;
        private readonly HttpClient mClient;
        private readonly ILogger<GithubClient> mLogger;

        public GithubClient(
            IConfiguration configuration,
            IHttpClientFactory clientFactory,
            ILogger<GithubClient> logger)
        {
            mConfiguration = configuration;
            mLogger = logger;
            mClient = clientFactory.CreateClient("GithubClient");
        }

        public async Task<string> GetPosts()
        {
            string responseString = string.Empty;
            try{
                responseString = await mClient.GetStringAsync(mConfiguration["codwiznet:Github:PostsFolderUrl"].ToString());
            }
            catch(Exception e)
            {
                return e.Message;
            }
            return responseString;
        }
    }
}