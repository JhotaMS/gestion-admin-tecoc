using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;
using gestionAdminTECOCApi.Infrastructure.Abstractions;

namespace gestionAdminTECOCApi.Application.Features.Auth.Login;

internal sealed class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IJwtTokenService jwtTokenService
) : ICommandHandler<LoginCommand, LoginResponse> {

    public async Task<Result<LoginResponse>> Handle( LoginCommand request, CancellationToken cancellationToken ) {
        var repo = unitOfWork.Repository<User>();
        var users = await repo.GetAsync( u => u.Email == request.Email, cancellationToken: cancellationToken );
        var user = users.FirstOrDefault();
        if (user is null) {
            return Result.Failure<LoginResponse>( new Error( "Auth.InvalidCredentials", "Usuario o contraseña inválidos" ) );
        }
        if (!PasswordHasher.Verify( user.PasswordHash, request.Password )) {
            return Result.Failure<LoginResponse>( new Error( "Auth.InvalidCredentials", "Usuario o contraseña inválidos" ) );
        }

        var token = jwtTokenService.GenerateToken( user.Id, user.Email, user.FullName, "user" );
        var userDto = new UserDto( user.Id, user.FullName, user.Email, "user" );

        return Result.Success( new LoginResponse( token, 3600, userDto ) );
    }
}
