using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SolicitorFinder.Mediator.Extensions;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Tests.Integration;

public sealed class MediatorIntegrationTests
{
    [Fact]
    public async Task Mediator_EndToEnd_WithDI_WorksCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddTransient<IRequestHandler<TestCommand, string>, TestCommandHandler>();
        services.AddTransient<INotificationHandler<TestEvent>, TestEventHandler>();

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        var commandResult = await mediator.Send(new TestCommand { Value = "Test" });

        // Assert
        commandResult.Should().Be("Handled: Test");
    }

    [Fact]
    public async Task Mediator_PublishEvent_NotifiesAllHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddTransient<INotificationHandler<TestEvent>, TestEventHandler>();
        services.AddTransient<INotificationHandler<TestEvent>, SecondTestEventHandler>();

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        TestEventHandler.CallCount = 0;
        SecondTestEventHandler.CallCount = 0;

        // Act
        await mediator.Publish(new TestEvent { Message = "Test Event" });

        // Assert
        TestEventHandler.CallCount.Should().Be(1);
        SecondTestEventHandler.CallCount.Should().Be(1);
    }

    private sealed class TestCommand : IRequest<string>
    {
        public string Value { get; set; } = string.Empty;
    }

    private sealed class TestCommandHandler : IRequestHandler<TestCommand, string>
    {
        public Task<string> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult($"Handled: {request.Value}");
        }
    }

    private sealed class TestEvent : INotification
    {
        public string Message { get; set; } = string.Empty;
    }

    private sealed class TestEventHandler : INotificationHandler<TestEvent>
    {
        public static int CallCount { get; set; }

        public Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.CompletedTask;
        }
    }

    private sealed class SecondTestEventHandler : INotificationHandler<TestEvent>
    {
        public static int CallCount { get; set; }

        public Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.CompletedTask;
        }
    }
}
