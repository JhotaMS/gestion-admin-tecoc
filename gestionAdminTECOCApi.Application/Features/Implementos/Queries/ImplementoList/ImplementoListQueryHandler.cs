using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Implementos;
using gestionAdminTECOCApi.Domain.Implementos.Dtos;

namespace gestionAdminTECOCApi.Application.Features.Implementos.Queries.ImplementoList;

internal sealed class ImplementoListQueryHandler(
        ImplementoService implementoService
    ) : IQueryHandler<ImplementoListQuery, ImplementoListResponse> {

    public async Task<Result<ImplementoListResponse>> Handle( ImplementoListQuery request
        , CancellationToken cancellationToken
    ) {
        IEnumerable<ImplementoDto> implementos = await implementoService.ImplementosDisponiblesAsync();

        string? mensaje = !implementos.Any()
            ? "No hay implementos disponibles"
            : null;

        return new ImplementoListResponse( implementos, mensaje );
    }
}
