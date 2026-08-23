using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Infrastructure.Extensions;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure( this IServiceCollection services ) {
        services.AddScoped<IJwtService, JwtService>();
        return services;
    }
}
