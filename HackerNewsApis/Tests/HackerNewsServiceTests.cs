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

        // Mocking cache and HttpClientFactory here
        // ...

        // Act
        var result = await _service.GetNewestStoriesAsync();

        // Assert
        Assert.NotEmpty(result);
    }
}
