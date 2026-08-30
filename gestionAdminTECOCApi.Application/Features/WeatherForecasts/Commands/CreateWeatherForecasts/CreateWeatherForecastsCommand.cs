using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.WeatherForecasts.Commands.CreateWeatherForecasts;

public record CreateWeatherForecastsCommand(
    ) : ICommand<CreateWeatherForecastsResponse>;
