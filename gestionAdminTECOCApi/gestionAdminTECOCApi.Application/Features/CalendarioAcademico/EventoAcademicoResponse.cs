namespace gestionAdminTECOCApi.Application.Features.CalendarioAcademico;

public record EventoAcademicoResponse(
    Guid Id,
    string Titulo,
    string? Descripcion,
    DateOnly FechaInicio,
    DateOnly? FechaFin,
    bool Enabled
);
