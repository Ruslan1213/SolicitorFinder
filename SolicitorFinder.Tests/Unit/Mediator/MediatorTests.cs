using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SolicitorFinder.Mediator;
using SolicitorFinder.Mediator.Exceptions;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Tests.Unit.Mediator;

public sealed class MediatorTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<SolicitorFinder.Mediator.Mediator>> _loggerMock;
    private readonly SolicitorFinder.Mediator.Mediator _sut;

    public MediatorTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<SolicitorFinder.Mediator.Mediator>>();
        _sut = new SolicitorFinder.Mediator.Mediator(_serviceProviderMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Send_WithValidRequest_CallsHandler()
    {
        // Arrange
        var request = new TestRequest();
        var handler = new TestRequestHandler();
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IRequestHandler<TestRequest, string>)))
            .Returns(handler);

        // Act
        var result = await _sut.Send(request);

        // Assert
        result.Should().Be("Test Response");
    }

    [Fact]
    public async Task Send_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        TestRequest? request = null;

        // Act
        Func<Task> act = async () => await _sut.Send<string>(request!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Send_WithNoHandler_ThrowsHandlerNotFoundException()
    {
        // Arrange
        var request = new TestRequest();
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IRequestHandler<TestRequest, string>)))
            .Returns(null);

        // Act
        Func<Task> act = async () => await _sut.Send(request);

        // Assert
        await act.Should().ThrowAsync<HandlerNotFoundException>();
    }

    [Fact]
    public async Task Publish_WithNullNotification_ThrowsArgumentNullException()
    {
        // Arrange
        TestNotification? notification = null;

        // Act
        Func<Task> act = async () => await _sut.Publish(notification!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    private sealed class TestRequest : IRequest<string> { }

    private sealed class TestRequestHandler : IRequestHandler<TestRequest, string>
    {
        public Task<string> Handle(TestRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult("Test Response");
        }
    }

    private sealed class TestNotification : INotification { }

    private sealed class TestNotificationHandler : INotificationHandler<TestNotification>
    {
        public bool WasCalled { get; private set; }

        public Task Handle(TestNotification notification, CancellationToken cancellationToken)
        {
            WasCalled = true;
            return Task.CompletedTask;
        }
    }
}
