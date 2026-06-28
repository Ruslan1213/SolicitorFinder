using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SolicitorFinder.Mediator.Exceptions;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Mediator;
public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Mediator>? _logger;

    public Mediator(IServiceProvider serviceProvider, ILogger<Mediator>? logger = null)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        _logger?.LogDebug("Sending request {RequestType} with response {ResponseType}",
            requestType.Name, responseType.Name);

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            throw new HandlerNotFoundException(requestType, responseType);
        }

        var handleMethod = handlerType.GetMethod("Handle");

        if (handleMethod == null)
        {
            throw new InvalidOperationException($"Handle method not found on {handlerType.Name}");
        }

        try
        {
            var result = handleMethod.Invoke(handler, new object[] { request, cancellationToken });

            return await (Task<TResponse>)result!;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling request {RequestType}", requestType.Name);

            throw;
        }
    }

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        _logger?.LogDebug("Sending void request {RequestType}", requestType.Name);
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            throw new HandlerNotFoundException(requestType);
        }

        var handleMethod = handlerType.GetMethod("Handle");
        if (handleMethod == null)
        {
            throw new InvalidOperationException($"Handle method not found on {handlerType.Name}");
        }

        try
        {
            var result = handleMethod.Invoke(handler, new object[] { request, cancellationToken });

            await (Task)result!;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling void request {RequestType}", requestType.Name);

            throw;
        }
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        var notificationType = notification.GetType();
        _logger?.LogDebug("Publishing notification {NotificationType}", notificationType.Name);
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
        var handlers = _serviceProvider.GetServices(handlerType).ToList();

        if (!handlers.Any())
        {
            _logger?.LogWarning("No handlers found for notification {NotificationType}", notificationType.Name);

            return;
        }

        var tasks = new List<Task>();
        var handleMethod = handlerType.GetMethod("Handle");

        if (handleMethod == null)
        {
            throw new InvalidOperationException($"Handle method not found on {handlerType.Name}");
        }

        foreach (var handler in handlers)
        {
            var task = (Task)handleMethod.Invoke(handler, new object[] { notification, cancellationToken })!;
            tasks.Add(task);
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error publishing notification {NotificationType}", notificationType.Name);

            throw;
        }
    }
}
