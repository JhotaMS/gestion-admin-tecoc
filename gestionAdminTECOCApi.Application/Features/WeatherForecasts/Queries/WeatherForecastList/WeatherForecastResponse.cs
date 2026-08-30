using gestionAdminTECOCApi.Domain.WeatherForecasts.Dtos;

namespace gestionAdminTECOCApi.Application.Features.WeatherForecasts.Queries.WeatherForecastList;

public record WeatherForecastResponse(
    IEnumerable<WeatherForecastDto> WeatherForecasts
);
