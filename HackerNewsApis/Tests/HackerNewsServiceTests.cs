using Moq;
using Xunit;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

public class HackerNewsServiceTests
{
    private readonly HackerNewsService _service;
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly Mock<IHttpClientFactory> _clientFactoryMock;

    public HackerNewsServiceTests()
    {
        _cacheMock = new Mock<IMemoryCache>();
        _clientFactoryMock = new Mock<IHttpClientFactory>();
        _service = new HackerNewsService(_cacheMock.Object, _clientFactoryMock.Object);
    }

   
    [Fact]
    public async Task GetNewestStoriesAsync_ReturnsStories()
    {
        // Arrange
        var stories = new List<Story> { new Story { Title = "Test", Url = "http://example.com" } };

        // Mocking cache behavior
        _ = _cacheMock.Setup(cache => cache.TryGetValue("newestStories", out It.Ref<List<Story>>.IsAny))
                      .Returns(false); // Simulate cache miss

        // Mocking HttpClient behavior
        var mockHttpClient = new Mock<HttpClient>();
        mockHttpClient.Setup(client => client.GetStringAsync(It.IsAny<string>()))
                      .ReturnsAsync("[1, 2, 3]"); // Simulate response from newstories.json
        mockHttpClient.SetupSequence(client => client.GetStringAsync(It.IsAny<string>()))
                      .ReturnsAsync("{\"title\": \"Test\", \"url\": \"http://example.com\"}") // Simulate response for item/1.json
                      .ReturnsAsync("{\"title\": \"Test2\", \"url\": \"http://example2.com\"}"); // Simulate response for item/2.json

        _clientFactoryMock.Setup(factory => factory.CreateClient())
                          .Returns(mockHttpClient.Object);

        // Act
        var result = await _service.GetNewestStoriesAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(stories.Count, result.Count); // Check if number of stories returned matches expected
        Assert.Equal(stories[0].Title, result[0].Title); // Check if the first story's title matches expected
        Assert.Equal(stories[0].Url, result[0].Url); // Check if the first story's URL matches expected
    }
}