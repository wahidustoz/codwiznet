using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using spa.Models.Github;

namespace spa.Services
{
    public interface IGithubClient
    {
        Task<List<GitContent>> GetFolderContents();
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

        public async Task<List<GitContent>> GetFolderContents()
        {
            List<GitContent> contents = new List<GitContent>();
            try
            {
                string responseString = await mClient.GetStringAsync(mConfiguration["codwiznet:Github:PostsFolderUrl"].ToString());
                if (string.IsNullOrWhiteSpace(responseString))
                {
                    throw new Exception("Got empty response from Github ...");
                }

                contents = JsonSerializer.Deserialize<List<GitContent>>(responseString);
            }
            catch (Exception e)
            {
                mLogger.LogError(e.Message);
            }

            return contents;
        }
    }
}