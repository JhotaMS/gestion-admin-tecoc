namespace gestionAdminTECOCApi.Domain.Implementos.Dtos;

public record ImplementoDto(
    Guid Id,
    string Nombre,
    string Codigo,
    string? Descripcion,
    int CantidadTotal,
    int CantidadDisponible,
    string Estado
);
