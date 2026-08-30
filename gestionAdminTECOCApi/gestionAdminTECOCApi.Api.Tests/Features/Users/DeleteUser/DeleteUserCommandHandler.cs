using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Users.DeleteUser;

internal sealed class DeleteUserCommandHandler(
    UserService userService
) : ICommandHandler<DeleteUserCommand> {

    public async Task<Result> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken
    ) => await userService.DeleteUserAsync( request.Id, cancellationToken );
}