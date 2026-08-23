using FluentValidation;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Users.CreateUser;

public sealed class UserCommandValidator : AbstractValidator<UserCommand> {
    private const int MaximumFullNameLength = 150;
    private const int MaximumPositionLength = 100;
    private const int MinimumDocumentNumberLength = 5;
    private const int MaximumDocumentNumberLength = 15;

    public UserCommandValidator() {
        RuleFor( command => command.FullName )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "El nombre completo es obligatorio" )
            .MaximumLength( MaximumFullNameLength )
            .WithMessage( $"El nombre completo no puede superar los {MaximumFullNameLength} caracteres" );

        RuleFor( command => command.DocumentType )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "El tipo de documento es obligatorio" )
            .Must( documentType => DocumentTypeCodes.IsAllowed( documentType ) )
            .WithMessage( command => UserErrors.DocumentTypeNotAllowed( command.DocumentType ).Name );

        RuleFor( command => command.DocumentNumber )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "El número de documento es obligatorio" )
            .Matches( "^[0-9]+$" )
            .WithMessage( "El número de documento solo puede contener dígitos" )
            .Length( MinimumDocumentNumberLength, MaximumDocumentNumberLength )
            .WithMessage( $"El número de documento debe tener entre {MinimumDocumentNumberLength} y {MaximumDocumentNumberLength} dígitos" );

        RuleFor( command => command.Position )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "El cargo es obligatorio" )
            .MaximumLength( MaximumPositionLength )
            .WithMessage( $"El cargo no puede superar los {MaximumPositionLength} caracteres" );
    }
}
