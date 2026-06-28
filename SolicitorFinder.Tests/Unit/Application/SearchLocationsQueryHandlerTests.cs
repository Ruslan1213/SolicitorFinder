using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SolicitorFinder.Application.Location.SearchLocation;
using SolicitorFinder.Services.Interfaces;
using SolicitorFinder.Services.Models;

namespace SolicitorFinder.Tests.Unit.Application;

public sealed class SearchLocationsQueryHandlerTests
{
    private readonly Mock<ILocationService> _locationServiceMock;
    private readonly Mock<ILogger<SearchLocationsQueryHandler>> _loggerMock;
    private readonly SearchLocationsQueryHandler _sut;

    public SearchLocationsQueryHandlerTests()
    {
        _locationServiceMock = new Mock<ILocationService>();
        _loggerMock = new Mock<ILogger<SearchLocationsQueryHandler>>();
        _sut = new SearchLocationsQueryHandler(_locationServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ReturnsLocationDtos()
    {
        // Arrange
        var query = new SearchLocationsQuery("London");
        var serviceResponse = new List<LocationResponse>
        {
            new LocationResponse { Title = "London", Text = "London, UK" },
            new LocationResponse { Title = "London", Text = "London, Ontario" }
        };

        _locationServiceMock
            .Setup(s => s.SearchLocationsAsync(query.SearchText, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.First().Title.Should().Be("London");
        result.First().Text.Should().Be("London, UK");
    }

    [Fact]
    public async Task Handle_WithEmptyResults_ReturnsEmptyList()
    {
        // Arrange
        var query = new SearchLocationsQuery("NonExistent");
        _locationServiceMock
            .Setup(s => s.SearchLocationsAsync(query.SearchText, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LocationResponse>());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CallsLocationService_WithCorrectParameters()
    {
        // Arrange
        var query = new SearchLocationsQuery("Test City");
        _locationServiceMock
            .Setup(s => s.SearchLocationsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LocationResponse>());

        // Act
        await _sut.Handle(query, CancellationToken.None);

        // Assert
        _locationServiceMock.Verify(
            s => s.SearchLocationsAsync("Test City", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
