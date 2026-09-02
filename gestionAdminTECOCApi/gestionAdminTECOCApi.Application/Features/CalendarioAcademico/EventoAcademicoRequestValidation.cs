using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.CalendarioAcademico;

namespace gestionAdminTECOCApi.Application.Features.CalendarioAcademico;

internal static class EventoAcademicoRequestValidation {
    public static Error? Validate(
        string? titulo,
        string? descripcion,
        DateOnly fechaInicio,
        DateOnly? fechaFin
    ) {
        if (string.IsNullOrWhiteSpace( titulo ))
            return EventoAcademicoErrors.TituloRequired;

        if (titulo.Trim().Length > EventoAcademico.MaximumTituloLength)
            return EventoAcademicoErrors.TituloTooLong;

        if (descripcion is not null && descripcion.Trim().Length > EventoAcademico.MaximumDescripcionLength)
            return EventoAcademicoErrors.DescripcionTooLong;

        if (fechaFin is not null && fechaFin.Value < fechaInicio)
            return EventoAcademicoErrors.FechaFinAnteriorAFechaInicio;

        return null;
    }
}
