using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Groups;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.Groups.GetAllGroups;

public record GetAllGroupsQuery : IQuery<GetAllGroupsResponse>;

public record GetAllGroupsResponse(
    IReadOnlyList<GroupResponse> Groups
);

internal sealed class GetAllGroupsQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetAllGroupsQuery, GetAllGroupsResponse> {
    public async Task<Result<GetAllGroupsResponse>> Handle(
        GetAllGroupsQuery request,
        CancellationToken cancellationToken
    ) {
        IReadOnlyList<Group> groups = await unitOfWork
            .Repository<Group>()
            .GetAllAsync( cancellationToken );

        IReadOnlyList<GroupResponse> response = groups
            .OrderBy( group => group.Name )
            .Select( group => new GroupResponse(
                group.Id,
                group.Name,
                group.Code,
                group.Enabled
            ) )
            .ToList();

        return Result.Success( new GetAllGroupsResponse( response ) );
    }
}
