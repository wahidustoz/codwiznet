using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using spa.Models.Github;

namespace spa.Services
{
    public interface IGithubClient
    {
        Task<List<GitContent>> GetFolderContents(string forlderUrl);
    }

    public class GithubClient : IGithubClient
    {
        private readonly HttpClient mClient;
        private readonly ILogger<GithubClient> mLogger;

        public GithubClient(
            IHttpClientFactory clientFactory,
            ILogger<GithubClient> logger)
        {
            mLogger = logger;
            mClient = clientFactory.CreateClient("GithubClient");
        }

        public async Task<List<GitContent>> GetFolderContents(string folderUrl)
        {
            List<GitContent> contents = new List<GitContent>();
            try
            {
                string responseString = await mClient.GetStringAsync(folderUrl);
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