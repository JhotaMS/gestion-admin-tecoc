using FluentValidation;
using gestionAdminTECOCApi.Domain.Loans;

namespace gestionAdminTECOCApi.Application.Features.Loans.CreateImplementoPrestado;

public sealed class CreateImplementoPrestadoCommandValidator : AbstractValidator<CreateImplementoPrestadoCommand> {
    private const int MaximumObservacionLength = 500;

    public CreateImplementoPrestadoCommandValidator() {
        RuleFor( command => command.UserId )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "El identificador del usuario/docente es obligatorio" )
            .NotEqual( Guid.Empty )
            .WithMessage( "El identificador del usuario/docente no es válido" );

        RuleFor( command => command.ImplementoId )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "El identificador del implemento es obligatorio" )
            .NotEqual( Guid.Empty )
            .WithMessage( "El identificador del implemento no es válido" );

        RuleFor( command => command.TipoRevisionId )
            .Cascade( CascadeMode.Stop )
            .GreaterThan( 0 )
            .WithMessage( "El tipo de revisión es obligatorio" );

        RuleFor( command => command.EstadoTipo )
            .Cascade( CascadeMode.Stop )
            .IsInEnum()
            .WithMessage( "El estado del implemento no es válido (debe ser Malo, Regular o Bueno)" );

        RuleFor( command => command.FechaInicio )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "La fecha de inicio es obligatoria" )
            .NotEqual( default( DateTime ) )
            .WithMessage( "La fecha de inicio no es válida" );

        RuleFor( command => command.FechaFin )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "La fecha de fin es obligatoria" )
            .NotEqual( default( DateTime ) )
            .WithMessage( "La fecha de fin no es válida" )
            .GreaterThanOrEqualTo( command => command.FechaInicio )
            .WithMessage( "La fecha de fin debe ser igual o posterior a la fecha de inicio" );

        When( command => !string.IsNullOrEmpty( command.Observacion ), () => {
            RuleFor( command => command.Observacion )
                .MaximumLength( MaximumObservacionLength )
                .WithMessage( $"La observación no puede superar los {MaximumObservacionLength} caracteres" );
        } );
    }
}

