using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Users.GetUsers;

public record GetUsersQuery : IQuery<GetUsersResponse>;

public record GetUsersResponse( IReadOnlyList<UserDto> Users );

public record UserDto(
    Guid Id,
    string FullName,
    string UserName,
    string Email,
    bool Enabled,
    GroupDto? Group
);

public record GroupDto(
    Guid Id,
    string Name,
    string Code
);

internal sealed class GetUsersQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetUsersQuery, GetUsersResponse> {
    public async Task<Result<GetUsersResponse>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken
    ) {
        var users = await unitOfWork
            .Repository<User>()
            .GetAsync(
                predicate: null,
                orderBy: null,
                includeString: "Group",
                disableTracking: true,
                cancellationToken: cancellationToken
            );

        var result = users
            .Select( user => new UserDto(
                user.Id,
                user.FullName,
                user.UserName,
                user.Email,
                user.Enabled,
                user.Group is null
                    ? null
                    : new GroupDto( user.Group.Id, user.Group.Name, user.Group.Code )
            ) )
            .ToList();

        return Result.Success( new GetUsersResponse( result ) );
    }
}
