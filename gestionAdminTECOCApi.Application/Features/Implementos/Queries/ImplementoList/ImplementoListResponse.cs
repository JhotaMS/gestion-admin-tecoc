using gestionAdminTECOCApi.Domain.Implementos.Dtos;

namespace gestionAdminTECOCApi.Application.Features.Implementos.Queries.ImplementoList;

public record ImplementoListResponse(
    IEnumerable<ImplementoDto> Implementos,
    string? Mensaje
);
