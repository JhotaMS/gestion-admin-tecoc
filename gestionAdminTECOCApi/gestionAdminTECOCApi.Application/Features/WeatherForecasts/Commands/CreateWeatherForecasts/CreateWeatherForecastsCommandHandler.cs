using gestionAdminTECOCApi.Application.Features.WeatherForecasts.Events.NotifyWeatherForecastCreated;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.WeatherForecasts;

namespace gestionAdminTECOCApi.Application.Features.WeatherForecasts.Commands.CreateWeatherForecasts;

internal sealed class CreateWeatherForecastsCommandHandler(
        IDispatch dispatch
        , IJsonConfiguration jsonConfiguration
        , WeatherForecastService forecastService
    ) : ICommandHandler<CreateWeatherForecastsCommand, CreateWeatherForecastsResponse> {

    public async Task<Result<CreateWeatherForecastsResponse>> Handle(
        CreateWeatherForecastsCommand request,
        CancellationToken cancellationToken
    ) {

        IEnumerable<WeatherForecast> weatherForecasts = await forecastService
            .GenerateForecastTimesAsync( cancellationToken );

        foreach (var forecast in weatherForecasts) {
            string serializer = jsonConfiguration.SerializeObject( forecast );
            await dispatch.Publish( new NotifyWeatherForecastCreatedCommand(
                serializer
            )
            , cancellationToken );
        }

        return new CreateWeatherForecastsResponse(
            weatherForecasts.Select( forecast => forecast.Id )
            .ToArray()
        );
    }
}
