using FluentValidation;
using MediatR;

namespace gestionAdminTECOCApi.Application.Abstractions.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> _validators
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> {

    public async Task<TResponse> Handle(
        TRequest requesxxxt,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    ) {

        return await next();
    }
}
