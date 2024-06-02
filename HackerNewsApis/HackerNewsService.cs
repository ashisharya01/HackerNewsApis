using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Text.Json;

public class HackerNewsService
{
    private readonly IMemoryCache _cache;
    private readonly IHttpClientFactory _clientFactory;
    private const string HackerNewsUrl = "https://hacker-news.firebaseio.com/v0/";

    public HackerNewsService(IMemoryCache cache, IHttpClientFactory clientFactory)
    {
        _cache = cache;
        _clientFactory = clientFactory;
    }

    public async Task<List<Story>> GetNewestStoriesAsync()
    {
        if (!_cache.TryGetValue("newestStories", out List<Story>? stories))
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync(HackerNewsUrl + "newstories.json");

            var storyIds = JsonSerializer.Deserialize<List<int>>(response);
            if (storyIds == null)
            {
                // Log or handle the null value
                return new List<Story>(); // Return an empty list if deserialization fails
            }

            stories = new List<Story>();

            foreach (var id in storyIds.Take(200))
            {
                try
                {
                    var storyResponse = await client.GetStringAsync(HackerNewsUrl + $"item/{id}.json");
                    var story = JsonSerializer.Deserialize<Story>(storyResponse);
                    if (story != null)
                    {
                        stories.Add(story);
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the exception
                }
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _cache.Set("newestStories", stories, cacheEntryOptions);
        }

        return stories ?? new List<Story>(); // Return stories or an empty list if null
    }
}
