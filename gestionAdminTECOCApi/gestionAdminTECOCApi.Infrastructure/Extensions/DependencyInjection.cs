using Microsoft.Extensions.DependencyInjection;
using gestionAdminTECOCApi.Infrastructure.Abstractions;
using gestionAdminTECOCApi.Infrastructure.Services;

namespace gestionAdminTECOCApi.Infrastructure.Extensions;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure( this IServiceCollection services ) {
        services.AddSingleton<IJwtTokenService>( new JwtTokenService() );
        return services;
    }
}
