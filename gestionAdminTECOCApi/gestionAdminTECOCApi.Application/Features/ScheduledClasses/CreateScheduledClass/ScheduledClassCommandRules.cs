using gestionAdminTECOCApi.Domain.ScheduledClasses;

namespace gestionAdminTECOCApi.Application.Features.ScheduledClasses.CreateScheduledClass;

public static class ScheduledClassCommandRules {
    public const int MaximumTopicLength = 200;
    public const int MaximumCourseLevelLength = 100;

    public static IReadOnlyList<string> Validate( ScheduledClassCommand command ) {
        ArgumentNullException.ThrowIfNull( command );

        List<string> messages = [];

        ValidateScheduledDate( command.ScheduledDate, messages );
        ValidateScheduledTime( command.ScheduledTime, messages );

        ValidateText(
            command.Topic
            , "El tema de la clase es obligatorio"
            , $"El tema de la clase no puede superar los {MaximumTopicLength} caracteres"
            , MaximumTopicLength
            , messages
        );

        ValidateText(
            command.CourseLevel
            , "El nivel o unidad del curso es obligatorio"
            , $"El nivel o unidad del curso no puede superar los {MaximumCourseLevelLength} caracteres"
            , MaximumCourseLevelLength
            , messages
        );

        return messages;
    }

    private static void ValidateScheduledDate( string? scheduledDate, List<string> messages ) {
        if (string.IsNullOrWhiteSpace( scheduledDate )) {
            messages.Add( "La fecha de la clase es obligatoria" );
            return;
        }

        if (!ClassScheduleFormats.TryParseDate( scheduledDate, out _ )) {
            messages.Add( ScheduledClassErrors.DateFormatNotAllowed( scheduledDate ).Name );
        }
    }

    private static void ValidateScheduledTime( string? scheduledTime, List<string> messages ) {
        if (string.IsNullOrWhiteSpace( scheduledTime )) {
            messages.Add( "La hora de la clase es obligatoria" );
            return;
        }

        if (!ClassScheduleFormats.TryParseTime( scheduledTime, out _ )) {
            messages.Add( ScheduledClassErrors.TimeFormatNotAllowed( scheduledTime ).Name );
        }
    }

    private static void ValidateText(
        string? value
        , string requiredMessage
        , string maximumLengthMessage
        , int maximumLength
        , List<string> messages
    ) {
        if (string.IsNullOrWhiteSpace( value )) {
            messages.Add( requiredMessage );
            return;
        }

        if (value.Trim().Length > maximumLength) {
            messages.Add( maximumLengthMessage );
        }
    }
}
