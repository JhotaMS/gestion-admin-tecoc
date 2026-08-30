using gestionAdminTECOCApi.Domain.WeatherForecasts.Dtos;

namespace gestionAdminTECOCApi.Application.Features.WeatherForecastsHistories.WeatherForecastById;

public record WeatherForecastHistoryByIdQueryResponse(
      Guid Id
    , WeatherForecastByIdDto? Proccess
    , DateOnly? CreatedDate
    , string? CreatedByUser
    , DateOnly? LastModifiedDate
    , string? LastModifiedByUser
);
