using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Groups;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;

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

        // Los cupos matriculados se calculan sobre usuarios habilitados: un usuario
        // deshabilitado (eliminado logicamente) ya no ocupa un cupo del grupo.
        IReadOnlyList<User> matriculados = await unitOfWork
            .Repository<User>()
            .GetAsync( user => user.GroupId != null && user.Enabled, cancellationToken );

        ILookup<Guid, User> matriculadosPorGrupo = matriculados.ToLookup( user => user.GroupId!.Value );

        IReadOnlyList<GroupResponse> response = groups
            .OrderBy( group => group.Name )
            .Select( group => {
                int cupoDisponible = Math.Max( 0, group.CupoTotal - matriculadosPorGrupo[group.Id].Count() );

                return new GroupResponse(
                    group.Id,
                    group.Name,
                    group.Code,
                    group.Enabled,
                    group.CupoTotal,
                    cupoDisponible
                );
            } )
            .ToList();

        return Result.Success( new GetAllGroupsResponse( response ) );
    }
}
