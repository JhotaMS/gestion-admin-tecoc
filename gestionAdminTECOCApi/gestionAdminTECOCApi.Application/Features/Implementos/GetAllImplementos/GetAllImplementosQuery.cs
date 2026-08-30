using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Implementos;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.Implementos.GetAllImplementos;

public record GetAllImplementosQuery : IQuery<GetAllImplementosResponse>;

public record GetAllImplementosResponse( IReadOnlyList<ImplementoDto> Implementos );

public record ImplementoDto(
    Guid ImplementoId,
    string Codigo,
    string Nombre,
    string Descripcion,
    int CantidadTotal,
    int CantidadDisponible,
    string Estado,
    bool Activo
);

internal sealed class GetAllImplementosQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetAllImplementosQuery, GetAllImplementosResponse> {

    public async Task<Result<GetAllImplementosResponse>> Handle( GetAllImplementosQuery request, CancellationToken cancellationToken ) {
        var repo = unitOfWork.Repository<Implemento>();
        var items = await repo.GetAllAsync( cancellationToken );

        var dtos = items
            .OrderBy( i => i.Nombre )
            .Select( i => new ImplementoDto(
                i.Id,
                i.Codigo,
                i.Nombre,
                i.Descripcion,
                i.CantidadTotal,
                i.CantidadDisponible,
                i.Estado,
                i.Enabled ) )
            .ToList();

        return Result.Success( new GetAllImplementosResponse( dtos ) );
    }
}
