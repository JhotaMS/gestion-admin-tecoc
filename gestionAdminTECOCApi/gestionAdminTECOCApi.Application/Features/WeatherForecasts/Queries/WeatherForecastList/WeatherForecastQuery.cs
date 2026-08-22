using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.WeatherForecasts.Queries.WeatherForecastList;

public record WeatherForecastQuery(
) : IQuery<WeatherForecastResponse>;
