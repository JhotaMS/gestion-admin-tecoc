using gestionAdminTECOCApi.Domain.Constants;
using gestionAdminTECOCApi.Domain.DomainService;
using gestionAdminTECOCApi.Domain.Implementos.Dtos;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Domain.Implementos;

[DomainService]
public class ImplementoService(
    IQueryWrapper queryWrapper
) {
    public async Task<IEnumerable<ImplementoDto>> ImplementosDisponiblesAsync() =>
        await queryWrapper
        .QueryAsync<ImplementoDto>(
            nameof( SqlQueriesConstants.ImplementosDisponibles )
        );
}
