using MediatR;
using System.Runtime.CompilerServices;

namespace gestionAdminTECOCApi.Application.Messaging;

public class Dispatch( IMediator mediator ) : IDispatch {
    private readonly IMediator _mediator = mediator;

    public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request
        , [EnumeratorCancellation] CancellationToken cancellationToken = default
    ) {
        IAsyncEnumerable<TResponse> responseStream = _mediator.CreateStream( request
            , cancellationToken
        );

        await foreach (var response in responseStream.WithCancellation( cancellationToken )) {
            yield return response;
        }
    }
    public async IAsyncEnumerable<object?> CreateStream(
        object request
        , [EnumeratorCancellation] CancellationToken cancellationToken = default
    ) {
        var responseStream = _mediator.CreateStream( request
            , cancellationToken
        );

        await foreach (var response in responseStream.WithCancellation( cancellationToken )) {
            yield return response;
        }
    }
    public Task Publish(
        object notification
        , CancellationToken cancellationToken = default
    ) => _mediator.Publish( notification, cancellationToken );

    public Task Publish<TNotification>(
        TNotification notification
        , CancellationToken cancellationToken = default
    ) where TNotification : INotification
        => _mediator.Publish( notification, cancellationToken );

    public Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request
        , CancellationToken cancellationToken = default
    ) => _mediator.Send( request, cancellationToken );

    public Task Send<TRequest>(
        TRequest request
        , CancellationToken cancellationToken = default
    ) where TRequest : IRequest
        => _mediator.Send( request, cancellationToken );

    public Task<object?> Send(
        object request
        , CancellationToken cancellationToken = default
    ) => _mediator.Send( request, cancellationToken );
}
