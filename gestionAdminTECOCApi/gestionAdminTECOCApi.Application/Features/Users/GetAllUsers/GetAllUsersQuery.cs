using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Users.GetAllUsers;

public record GetAllUsersQuery : IQuery<GetAllUsersResponse>;

public record GetAllUsersResponse( IReadOnlyList<UserDto> Users );

public record UserDto(
    Guid UserId,
    string FullName,
    string DocumentType,
    string DocumentNumber,
    string UserName,
    string Email
);

internal sealed class GetAllUsersQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetAllUsersQuery, GetAllUsersResponse> {

    public async Task<Result<GetAllUsersResponse>> Handle( GetAllUsersQuery request, CancellationToken cancellationToken ) {
        var repo = unitOfWork.Repository<User>();
        var users = await repo.GetAllAsync( cancellationToken );

        var dtos = users
            .Select( user => new UserDto(
                user.Id,
                user.FullName,
                DocumentTypeCodes.ToCode( user.DocumentType ),
                user.DocumentNumber,
                user.UserName,
                user.Email
            ) )
            .ToList();

        return Result.Success( new GetAllUsersResponse( dtos ) );
    }
}
