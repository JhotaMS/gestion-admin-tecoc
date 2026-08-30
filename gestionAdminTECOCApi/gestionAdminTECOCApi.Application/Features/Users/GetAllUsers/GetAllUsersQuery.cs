using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Users.GetAllUsers;

public record GetAllUsersQuery : IQuery<GetAllUsersResponse>;

public record GetAllUsersResponse(
    IReadOnlyList<UserSummaryDto> Users
);

public record UserSummaryDto(
    Guid Id,
    string FullName,
    string UserName,
    string Email,
    string DocumentType,
    string DocumentNumber,
    bool Enabled,
    GroupDto? Group
);

public record GroupDto(
    Guid Id,
    string Name,
    string Code
);

internal sealed class GetAllUsersQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetAllUsersQuery, GetAllUsersResponse> {

    public async Task<Result<GetAllUsersResponse>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken
    ) {
        var repo = unitOfWork.Repository<User>();
        var items = await repo.GetAsync(
            predicate: null,
            orderBy: null,
            includeString: "Group",
            disableTracking: true,
            cancellationToken: cancellationToken
        );

        var dtos = items
            .Select( u => new UserSummaryDto(
                u.Id,
                u.FullName,
                u.UserName,
                u.Email,
                DocumentTypeCodes.ToDescription( u.DocumentType ),
                u.DocumentNumber,
                u.Enabled,
                u.Group is null
                    ? null
                    : new GroupDto( u.Group.Id, u.Group.Name, u.Group.Code )
            ) )
            .ToList();

        return Result.Success( new GetAllUsersResponse( dtos ) );
    }
}
