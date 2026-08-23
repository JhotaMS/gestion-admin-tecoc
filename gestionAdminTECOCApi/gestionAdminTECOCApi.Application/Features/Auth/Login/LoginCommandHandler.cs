using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Auth.Login;

internal sealed class LoginCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<LoginCommand, LoginResponse> {

    public async Task<Result<LoginResponse>> Handle( LoginCommand request, CancellationToken cancellationToken ) {
        var repo = unitOfWork.Repository<User>();
        var users = await repo.GetAsync( u => u.UserName == request.UserName, cancellationToken: cancellationToken );
        var user = users.FirstOrDefault();
        if (user is null) {
            return Result.Failure<LoginResponse>( new Error( "Auth.InvalidCredentials", "Usuario o contraseña inválidos" ) );
        }
        if (!PasswordHasher.Verify( user.PasswordHash, request.Password )) {
            return Result.Failure<LoginResponse>( new Error( "Auth.InvalidCredentials", "Usuario o contraseña inválidos" ) );
        }

        return Result.Success( new LoginResponse( user.Id, user.UserName, user.Email ) );
    }
}
