using gestionAdminTECOCApi.Domain.Loans;

namespace gestionAdminTECOCApi.Application.Features.Loans.CreateImplementoPrestado;

public record CreateImplementoPrestadoResponse(
    Guid Id,
    Guid UserId,
    Guid ImplementoId,
    int TipoRevisionId,
    string EstadoTipo,
    DateTime FechaInicio,
    DateTime FechaFin,
    string? Observacion
);

