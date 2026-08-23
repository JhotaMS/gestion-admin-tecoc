using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.ScheduledClasses;

public static class ScheduledClassErrors {
    public static Error ValidationFailed( IEnumerable<string> messages ) => new(
        "ScheduledClass.ValidationFailed",
        string.Join( " ", messages )
    );

    public static Error DateFormatNotAllowed( string? scheduledDate ) => new(
        "ScheduledClass.DateFormatNotAllowed",
        $"La fecha '{scheduledDate}' no corresponde al formato configurado en el sistema ({ClassScheduleFormats.DateFormat})"
    );

    public static Error TimeFormatNotAllowed( string? scheduledTime ) => new(
        "ScheduledClass.TimeFormatNotAllowed",
        $"La hora '{scheduledTime}' no corresponde al formato configurado en el sistema ({ClassScheduleFormats.TimeFormat})"
    );

    public static Error ScheduleAlreadyTaken( string scheduledDate, string scheduledTime ) => new(
        "ScheduledClass.ScheduleAlreadyTaken",
        $"Ya existe una clase programada para la fecha '{scheduledDate}' a las '{scheduledTime}'"
    );
}
