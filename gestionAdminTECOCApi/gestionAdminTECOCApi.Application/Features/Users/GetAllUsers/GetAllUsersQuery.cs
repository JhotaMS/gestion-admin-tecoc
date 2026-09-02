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
    GroupDto? Group,
    ProgramaAcademicoDto? ProgramaAcademico
);

public record GroupDto(
    Guid Id,
    string Name,
    string Code
);

public record ProgramaAcademicoDto(
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
            includes: new List<System.Linq.Expressions.Expression<Func<User, object>>> {
                u => u.Group!,
                u => u.ProgramaAcademico!
            },
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
                    : new GroupDto( u.Group.Id, u.Group.Name, u.Group.Code ),
                u.ProgramaAcademico is null
                    ? null
                    : new ProgramaAcademicoDto( u.ProgramaAcademico.Id, u.ProgramaAcademico.Name, u.ProgramaAcademico.Code )
            ) )
            .ToList();

        return Result.Success( new GetAllUsersResponse( dtos ) );
    }
}
