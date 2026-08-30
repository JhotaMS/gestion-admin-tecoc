using FluentValidation;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Users.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand> {
    private const int MaximumFullNameLength = 150;
    private const int MinimumDocumentNumberLength = 5;
    private const int MaximumDocumentNumberLength = 15;
    private const int MaximumUserNameLength = 50;
    private const int MaximumEmailLength = 150;

    public UpdateUserCommandValidator() {
        RuleFor( command => command.Id )
            .NotEmpty()
            .WithMessage( "El identificador del usuario es obligatorio" );

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

        RuleFor( command => command.UserName )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "El nombre de usuario es obligatorio" )
            .MaximumLength( MaximumUserNameLength )
            .WithMessage( $"El nombre de usuario no puede superar los {MaximumUserNameLength} caracteres" );

        RuleFor( command => command.Email )
            .Cascade( CascadeMode.Stop )
            .NotEmpty()
            .WithMessage( "El correo electrónico es obligatorio" )
            .MaximumLength( MaximumEmailLength )
            .WithMessage( $"El correo electrónico no puede superar los {MaximumEmailLength} caracteres" )
            .EmailAddress()
            .WithMessage( "El correo electrónico no tiene un formato válido" );
    }
}
