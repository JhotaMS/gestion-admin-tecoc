using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Auth.Login;

internal sealed class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtService jwtService
) : ICommandHandler<LoginCommand, LoginResponse> {

    public async Task<Result<LoginResponse>> Handle( LoginCommand request, CancellationToken cancellationToken ) {
        var repo = unitOfWork.Repository<User>();
        var users = await repo.GetAsync( u => u.Email == request.Email, cancellationToken: cancellationToken );
        var user = users.FirstOrDefault();
        if (user is null) {
            return Result.Failure<LoginResponse>( new Error( "Auth.InvalidCredentials", "Usuario o contraseña inválidos" ) );
        }
        if (user.IsLocked) {
            return Result.Failure<LoginResponse>( new Error( "Auth.AccountLocked", $"Cuenta bloqueada hasta {user.LockedUntil:O}. Intente más tarde." ) );
        }
        if (!passwordHasher.Verify( user.PasswordHash, request.Password )) {
            user.RecordFailedLogin();
            await unitOfWork.SaveChangesAsync();
            // TODO: Outbox mail to user when locked (TDDSIVI-109)
            return Result.Failure<LoginResponse>( new Error( "Auth.InvalidCredentials", "Usuario o contraseña inválidos" ) );
        }
        user.ResetFailedLogins();
        await unitOfWork.SaveChangesAsync();

        var accessToken = jwtService.GenerateAccessToken( user.Id, user.Email, Array.Empty<string>() );
        var refreshToken = jwtService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes( 15 );

        return Result.Success( new LoginResponse( accessToken, refreshToken, expiresAt, user.Id, user.Email ) );
    }
}
