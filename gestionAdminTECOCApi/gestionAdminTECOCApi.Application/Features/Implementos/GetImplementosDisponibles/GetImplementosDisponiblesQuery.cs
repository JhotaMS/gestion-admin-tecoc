using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Implementos;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.Implementos.GetImplementosDisponibles;

public record GetImplementosDisponiblesQuery : IQuery<GetImplementosDisponiblesResponse>;

public record GetImplementosDisponiblesResponse(
    IReadOnlyList<ImplementoDisponibleDto> Implementos,
    string? Mensaje
);

public record ImplementoDisponibleDto(
    Guid Id,
    string Nombre,
    string Codigo,
    string Descripcion,
    int CantidadTotal,
    int CantidadDisponible,
    string Estado
);

internal sealed class GetImplementosDisponiblesQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetImplementosDisponiblesQuery, GetImplementosDisponiblesResponse> {

    private const string SinDisponibles = "No hay implementos disponibles";

    public async Task<Result<GetImplementosDisponiblesResponse>> Handle( GetImplementosDisponiblesQuery request, CancellationToken cancellationToken ) {
        var repo = unitOfWork.Repository<Implemento>();
        var items = await repo.GetAsync( i => i.CantidadDisponible > 0 && i.Enabled, cancellationToken: cancellationToken );

        var dtos = items
            .OrderBy( i => i.Nombre )
            .Select( i => new ImplementoDisponibleDto(
                i.Id,
                i.Nombre,
                i.Codigo,
                i.Descripcion,
                i.CantidadTotal,
                i.CantidadDisponible,
                i.Estado ) )
            .ToList();

        return Result.Success( new GetImplementosDisponiblesResponse(
            dtos,
            dtos.Count == 0 ? SinDisponibles : null ) );
    }
}
