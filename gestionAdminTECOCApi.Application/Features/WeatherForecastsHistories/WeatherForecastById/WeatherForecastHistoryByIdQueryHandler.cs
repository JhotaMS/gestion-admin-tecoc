using gestionAdminTECOCApi.Application.Exceptions;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.WeatherForecasts.Dtos;
using gestionAdminTECOCApi.Domain.WeatherForecastsHistories;

namespace gestionAdminTECOCApi.Application.Features.WeatherForecastsHistories.WeatherForecastById;

internal sealed record class WeatherForecastHistoryByIdQueryHandler(
          WeatherForecastsHistoryService WeatherForecastsHistoryService
        , IJsonConfiguration JsonConfiguration
    ) : IQueryHandler<WeatherForecastHistoryByIdQuery, WeatherForecastHistoryByIdQueryResponse> {

    public async Task<Result<WeatherForecastHistoryByIdQueryResponse>> Handle(
          WeatherForecastHistoryByIdQuery request
        , CancellationToken cancellationToken
    ) {
        WeatherForecastsHistory history = await WeatherForecastsHistoryService
            .GetByAsync(
                  request.Id
                , cancellationToken
            );

        if (history is null)
            throw new NotFoundException( $"Producto con ID {request.Id} no encontrado." );

        var result = new WeatherForecastHistoryByIdQueryResponse(
            history.Id
            , JsonConfiguration.DeserializeObject<WeatherForecastByIdDto>( history.Proccess! )
            , history.CreatedDate
            , history.CreatedByUser
            , history.LastModifiedDate
            , history.LastModifiedByUser
        );

        return result;
    }
}
