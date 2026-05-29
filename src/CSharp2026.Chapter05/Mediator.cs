// -----------------------------------------------------------------------
//   Mediator.cs — Chapter 5 / Mediator Pattern (minimal)
//
//   A tiny in-process mediator showing the dispatch shape. Production
//   code uses the MediatR library (see Chapter 7), but the pattern is
//   small enough to demonstrate without a dependency.
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace CSharp2026.Chapter05.Mediator;

/// <summary>Marker for a request that produces a response of <typeparamref name="TResponse"/>.</summary>
public interface IRequest<TResponse> { }

/// <summary>Handler for a specific <see cref="IRequest{TResponse}"/>.</summary>
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IMediator
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Resolves a handler from DI by closing the open-generic interface over
/// the request's runtime type. Cached reflection would replace
/// <c>MakeGenericType</c> in production code.
/// </summary>
public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        object handler = serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No handler registered for {request.GetType().Name}.");

        System.Reflection.MethodInfo method =
            handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.HandleAsync))!;
        Task<TResponse> task = (Task<TResponse>)method.Invoke(handler, [request, cancellationToken])!;
        return await task.ConfigureAwait(false);
    }
}

public static class MediatorServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleMediator(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped<IMediator, Mediator>();
        return services;
    }
}
