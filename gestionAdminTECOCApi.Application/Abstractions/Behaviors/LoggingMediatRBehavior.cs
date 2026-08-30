using gestionAdminTECOCApi.Application.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace gestionAdminTECOCApi.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<TRequest> _logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand {

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
        ) {

        try {
            var name = request.GetType().Name;
            _logger.LogInformation(
                $"Ejecutando el command request: {name}"
            );

            var result = await next();

            _logger.LogInformation(
                $"El comando {name} se ejecuto exitosamente"
            );

            return result;
        }
        catch (Exception exception) {
            _logger.LogInformation(
                "Errors request {@requestName} {@DateTimeUtc} {@Message}",
                typeof( TRequest ).Name,
                DateTime.UtcNow,
                exception.Message
            );

            throw;
        }
    }
}
