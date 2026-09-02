using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.CalendarioAcademico.CreateEventoAcademico;

public record CreateEventoAcademicoCommand(
    string Titulo,
    string? Descripcion,
    DateOnly FechaInicio,
    DateOnly? FechaFin
) : ICommand<EventoAcademicoResponse>;
