using gestionAdminTECOCApi.Domain.Constants;
using gestionAdminTECOCApi.Domain.DomainService;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.WeatherForecasts.Dtos;

namespace gestionAdminTECOCApi.Domain.WeatherForecasts;

[DomainService]
public class WeatherForecastService(
    IQueryWrapper queryWrapper,
    IUnitOfWork unitOfWork
) {
    public async Task<IEnumerable<WeatherForecast>> GenerateForecastTimesAsync(
        CancellationToken cancellationToken
    ) {
        IEnumerable<WeatherForecast> weatherForecastList = WeatherForecastList();
        ArgumentNullException.ThrowIfNull( nameof( weatherForecastList ) );

        await unitOfWork.Repository<WeatherForecast>()
            .AddAsync( weatherForecastList, cancellationToken );

        return weatherForecastList;
    }

    public async Task<IEnumerable<WeatherForecastDto>> WeatherForecastsAsync() =>
        await queryWrapper
        .QueryAsync<WeatherForecastDto>(
            nameof( SqlQueriesConstants.AllWeastherForecasts )
        );

    public static IEnumerable<WeatherForecast> WeatherForecastList() {
        string[] sumaries = WeatherForecast.Summaries();
        return Enumerable
            .Range( 1, 5 )
            .Select( index => WeatherForecast.Create(
                DateOnly.FromDateTime( DateTime.Now.AddDays( index ) ),
                Random.Shared.Next( -20, 55 ),
                sumaries[Random.Shared.Next( sumaries.Length )]
            ) ).ToArray();
    }
}
