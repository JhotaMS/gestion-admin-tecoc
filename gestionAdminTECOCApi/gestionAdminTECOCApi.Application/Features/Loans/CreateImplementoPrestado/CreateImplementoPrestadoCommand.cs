using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Loans;
using System.ComponentModel.DataAnnotations;

namespace gestionAdminTECOCApi.Application.Features.Loans.CreateImplementoPrestado;

public record CreateImplementoPrestadoCommand(
    [Required] Guid UserId,
    [Required] Guid ImplementoId,
    [Required] int TipoRevisionId,
    [Required] EstadoTipoImplemento EstadoTipo,
    [Required] DateTime FechaInicio,
    [Required] DateTime FechaFin,
    string? Observacion
) : ICommand<CreateImplementoPrestadoResponse>;

