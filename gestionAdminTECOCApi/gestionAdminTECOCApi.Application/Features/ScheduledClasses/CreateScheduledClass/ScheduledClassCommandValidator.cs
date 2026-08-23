using FluentValidation;
using gestionAdminTECOCApi.Domain.Helpers;
using gestionAdminTECOCApi.Domain.ScheduledClasses;

namespace gestionAdminTECOCApi.Application.Features.ScheduledClasses.CreateScheduledClass;

public sealed class ScheduledClassCommandValidator : AbstractValidator<ScheduledClassCommand> {
    private const int MaximumTopicLength = 200;
    private const int MaximumCourseLevelLength = 100;

    public ScheduledClassCommandValidator() {
        RuleFor( command => command.ScheduledDate )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "La fecha de la clase es obligatoria" )
            .Must( scheduledDate => ClassScheduleFormats.TryParseDate( scheduledDate, out _ ) )
            .WithMessage( command => ScheduledClassErrors.DateFormatNotAllowed( command.ScheduledDate ).Name )
            .Must( NoEsAnteriorAHoy )
            .WithMessage( "La fecha de la clase no puede ser anterior a la fecha actual" );

        RuleFor( command => command.ScheduledTime )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "La hora de la clase es obligatoria" )
            .Must( scheduledTime => ClassScheduleFormats.TryParseTime( scheduledTime, out _ ) )
            .WithMessage( command => ScheduledClassErrors.TimeFormatNotAllowed( command.ScheduledTime ).Name );

        RuleFor( command => command.Topic )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "El tema de la clase es obligatorio" )
            .MaximumLength( MaximumTopicLength )
            .WithMessage( $"El tema de la clase no puede superar los {MaximumTopicLength} caracteres" );

        RuleFor( command => command.CourseLevel )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "El nivel o unidad del curso es obligatorio" )
            .MaximumLength( MaximumCourseLevelLength )
            .WithMessage( $"El nivel o unidad del curso no puede superar los {MaximumCourseLevelLength} caracteres" );
    }

    private static bool NoEsAnteriorAHoy( string scheduledDate )
        => ClassScheduleFormats.TryParseDate( scheduledDate, out DateOnly date )
        && date >= DateOnly.FromDateTime( DateTime.Now.ZoneByIdPacificStandardTime() );
}
