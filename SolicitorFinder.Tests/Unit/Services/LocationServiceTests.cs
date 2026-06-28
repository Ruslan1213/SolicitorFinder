using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SolicitorFinder.Services.Configurations;
using SolicitorFinder.Services.Services;
using System.Net;
using System.Text.Json;

namespace SolicitorFinder.Tests.Unit.Services;

public sealed class LocationServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly ScraperConfiguration _config;
    private readonly Mock<ILogger<LocationService>> _loggerMock;
    private readonly LocationService _sut;

    public LocationServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _config = new ScraperConfiguration
        {
            LocationBaseUrl = "https://api.test.com/locations?q={0}"
        };
        var options = Options.Create(_config);
        _loggerMock = new Mock<ILogger<LocationService>>();
        _sut = new LocationService(_httpClient, options, _loggerMock.Object);
    }

    [Fact]
    public async Task SearchLocationsAsync_WithValidQuery_ReturnsLocations()
    {
        // Arrange
        var query = "London";
        var expectedLocations = new[]
        {
            new { Title = "London", Text = "London, UK" },
            new { Title = "London", Text = "London, Ontario" }
        };
        var json = JsonSerializer.Serialize(expectedLocations);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });

        // Act
        var result = await _sut.SearchLocationsAsync(query);

        // Assert
        result.Should().HaveCount(2);
        result.First().Title.Should().Be("London");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a")]
    public async Task SearchLocationsAsync_WithInvalidQuery_ReturnsEmptyList(string query)
    {
        // Act
        var result = await _sut.SearchLocationsAsync(query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchLocationsAsync_WhenHttpRequestFails_ReturnsEmptyList()
    {
        // Arrange
        var query = "London";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _sut.SearchLocationsAsync(query);

        // Assert
        result.Should().BeEmpty();
    }
}
