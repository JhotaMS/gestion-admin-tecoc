using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.CalendarioAcademico.UpdateEventoAcademico;

public record UpdateEventoAcademicoCommand(
    Guid EventoAcademicoId,
    string Titulo,
    string? Descripcion,
    DateOnly FechaInicio,
    DateOnly? FechaFin
) : ICommand<EventoAcademicoResponse>;
