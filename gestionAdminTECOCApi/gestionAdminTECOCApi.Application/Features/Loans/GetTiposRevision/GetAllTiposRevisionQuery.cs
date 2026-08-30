using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Loans;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.Loans.GetTiposRevision;

public record GetAllTiposRevisionQuery : IQuery<GetAllTiposRevisionResponse>;

public record GetAllTiposRevisionResponse(
    IReadOnlyList<TipoRevisionDto> TiposRevision
);

public record TipoRevisionDto(
    int Id,
    string Nombre,
    string Descripcion
);

internal sealed class GetAllTiposRevisionQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetAllTiposRevisionQuery, GetAllTiposRevisionResponse> {

    public async Task<Result<GetAllTiposRevisionResponse>> Handle(
        GetAllTiposRevisionQuery request,
        CancellationToken cancellationToken
    ) {
        var repo = unitOfWork.Repository<TipoRevision>();
        var items = await repo.GetAllAsync( cancellationToken );

        var dtos = items
            .Select( t => new TipoRevisionDto( t.Id, t.Nombre, t.Descripcion ) )
            .ToList();

        return Result.Success( new GetAllTiposRevisionResponse( dtos ) );
    }
}

