using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SolicitorFinder.GeneralParser.Interfaces;
using SolicitorFinder.GeneralParser.Models;
using SolicitorFinder.Services.Configurations;
using SolicitorFinder.Services.Services;
using System.Net;

namespace SolicitorFinder.Tests.Integration;

public sealed class AreaServiceIntegrationTests
{
    private static HttpClient CreateHttpClient(string responseHtml)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseHtml)
            });

        return new HttpClient(handlerMock.Object);
    }

    private static ParseResult BuildParseResult(bool success, List<PageElement>? elements = null) =>
        new() { Success = success, Elements = elements ?? new List<PageElement>() };

    [Fact]
    public async Task GetAreasAsync_WithMockedParser_ReturnsAreas()
    {
        // Arrange
        var parseResult = BuildParseResult(true, new List<PageElement>
        {
            new()
            {
                TagName = "select",
                Children = new List<PageElement>
                {
                    new() { TagName = "option", InnerText = "Area 1", Attributes = new Dictionary<string, string> { { "value", "1" } } },
                    new() { TagName = "option", InnerText = "Area 2", Attributes = new Dictionary<string, string> { { "value", "2" } } }
                }
            }
        });

        var parserMock = new Mock<IPageParser>();
        parserMock
            .Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<ParseOptions>()))
            .Returns(parseResult);

        var sut = new AreaService(
            CreateHttpClient("<html></html>"),
            parserMock.Object,
            Options.Create(new ScraperConfiguration { BaseUrl = "https://test.com" }),
            new Mock<ILogger<AreaService>>().Object);

        // Act
        var result = await sut.GetAreasAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Id.Should().Be("1");
        result.First().Name.Should().Be("Area 1");
        result.Last().Id.Should().Be("2");
        result.Last().Name.Should().Be("Area 2");
    }

    [Fact]
    public async Task GetAreasAsync_WhenParserFails_ReturnsEmptyList()
    {
        // Arrange
        var parserMock = new Mock<IPageParser>();
        parserMock
            .Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<ParseOptions>()))
            .Returns(BuildParseResult(false));

        var sut = new AreaService(
            CreateHttpClient("<html></html>"),
            parserMock.Object,
            Options.Create(new ScraperConfiguration { BaseUrl = "https://test.com" }),
            new Mock<ILogger<AreaService>>().Object);

        // Act
        var result = await sut.GetAreasAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAreasAsync_WhenHttpFails_ReturnsEmptyList()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var parserMock = new Mock<IPageParser>();

        var sut = new AreaService(
            new HttpClient(handlerMock.Object),
            parserMock.Object,
            Options.Create(new ScraperConfiguration { BaseUrl = "https://test.com" }),
            new Mock<ILogger<AreaService>>().Object);

        // Act
        var result = await sut.GetAreasAsync();

        // Assert
        result.Should().BeEmpty();
        parserMock.Verify(p => p.Parse(It.IsAny<string>(), It.IsAny<ParseOptions>()), Times.Never);
    }

    [Fact]
    public async Task GetAreasAsync_WithEmptySelectElement_ReturnsEmptyList()
    {
        // Arrange
        var parserMock = new Mock<IPageParser>();
        parserMock
            .Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<ParseOptions>()))
            .Returns(BuildParseResult(true));

        var sut = new AreaService(
            CreateHttpClient("<html></html>"),
            parserMock.Object,
            Options.Create(new ScraperConfiguration { BaseUrl = "https://test.com" }),
            new Mock<ILogger<AreaService>>().Object);

        // Act
        var result = await sut.GetAreasAsync();

        // Assert
        result.Should().BeEmpty();
    }
}
