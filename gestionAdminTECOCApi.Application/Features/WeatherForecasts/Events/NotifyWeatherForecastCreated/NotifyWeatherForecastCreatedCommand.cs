using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.WeatherForecasts.Events.NotifyWeatherForecastCreated;

public record NotifyWeatherForecastCreatedCommand(
    string Proccess
) : INotify;
