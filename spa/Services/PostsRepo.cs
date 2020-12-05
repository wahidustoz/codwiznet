// David Wahid
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using spa.Models;

namespace spa.Services
{
    public class PostsRepo
    {
        public ILogger<PostsRepo> mLogger { get; }
        public IGithubClient mGitClient { get; }
        public IConfiguration mConfiguration { get; }

        readonly string mPostsUrl = string.Empty;

        public PostsRepo(ILogger<PostsRepo> logger, IGithubClient gitClient, IConfiguration configuration)
        {
            mLogger = logger;
            mGitClient = gitClient;
            mConfiguration = configuration;
            mPostsUrl = mConfiguration["codwiznet:Github:PostsFolderUrl"];
        }


        public async Task<List<Post>> GetPostsAsync()
        {
            var postsFolders = await mGitClient.GetFolderContents(mPostsUrl);

            if (postsFolders == null)
            {
                mLogger.LogError($"Error while loading posts. Could not read posts folder...");
                return new List<Post>();
            }

            var posts = new List<Post>();

            foreach (var postFolder in postsFolders)
            {
                if (string.Compare(postFolder.Type, "dir", System.StringComparison.OrdinalIgnoreCase) != 0)
                {
                    continue;
                }

                var postContents = await mGitClient.GetFolderContents($"{mPostsUrl}/{postFolder.Name}");

                if (postContents == null)
                {
                    mLogger.LogError($"Error while loading posts. Could not read post contents...");
                    return new List<Post>();
                }

                var titleContent = postContents.Where(p =>
                    p.Type == "file"
                    && p.Name.Contains("title", System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                var descriptionContent = postContents.Where(p =>
                    p.Type == "file"
                    && p.Name.Contains("description", System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                var contentContent = postContents.Where(p =>
                    p.Type == "file"
                    && p.Name.Contains("content", System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                var tagsContent = postContents.Where(p =>
                    p.Type == "file"
                    && p.Name.Contains("tags", System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                var headerImageContent = postContents.Where(p =>
                    p.Type == "file"
                    && p.Name.Contains("headerimage", System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();


                posts.Add(new Post()
                {
                    TitleUrl = titleContent?.DownloadUrl,
                    DescriptionUrl = descriptionContent?.DownloadUrl,
                    TagsUrl = tagsContent?.DownloadUrl,
                    ContentUrl = contentContent?.DownloadUrl,
                    HeaderImageUrl = headerImageContent?.DownloadUrl,
                    CreatedAt = DateTime.Parse(postFolder.Name)
                });
            }

            return posts;
        }
    }
}
