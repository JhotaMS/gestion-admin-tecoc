using FluentValidation;

namespace gestionAdminTECOCApi.Application.Features.Users.DeleteUser;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand> {
    public DeleteUserCommandValidator() {
        RuleFor( command => command.Id )
            .NotEmpty()
            .WithMessage( "El identificador del usuario es obligatorio" );
    }
}