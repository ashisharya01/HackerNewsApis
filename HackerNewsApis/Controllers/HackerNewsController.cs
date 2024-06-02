using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public HackerNewsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("newest")]
        public async Task<IActionResult> GetNewestStories()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://hacker-news.firebaseio.com/v0/newstories.json");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var storyIds = JsonSerializer.Deserialize<List<int>>(content);

                    var stories = new List<Story>();

                    foreach (var storyId in storyIds)
                    {
                        var storyResponse = await _httpClient.GetAsync($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json");

                        if (storyResponse.IsSuccessStatusCode)
                        {
                            var storyContent = await storyResponse.Content.ReadAsStringAsync();
                            var story = JsonSerializer.Deserialize<Story>(storyContent);
                            stories.Add(story);
                        }
                    }

                    return Ok(stories);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Failed to fetch newest stories from Hacker News API.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }

    public class Story
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
