using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.CalendarioAcademico;

public static class EventoAcademicoErrors {
    public static readonly Error TituloRequired = new(
        "EventoAcademico.TituloRequired",
        "El título del evento es obligatorio"
    );

    public static readonly Error TituloTooLong = new(
        "EventoAcademico.TituloTooLong",
        $"El título del evento no puede superar los {EventoAcademico.MaximumTituloLength} caracteres"
    );

    public static readonly Error DescripcionTooLong = new(
        "EventoAcademico.DescripcionTooLong",
        $"La descripción del evento no puede superar los {EventoAcademico.MaximumDescripcionLength} caracteres"
    );

    public static readonly Error FechaFinAnteriorAFechaInicio = new(
        "EventoAcademico.FechaFinAnteriorAFechaInicio",
        "La fecha de fin no puede ser anterior a la fecha de inicio"
    );

    public static readonly Error NotFound = new(
        "EventoAcademico.NotFound",
        "No se encontró el evento del calendario académico solicitado"
    );
}
