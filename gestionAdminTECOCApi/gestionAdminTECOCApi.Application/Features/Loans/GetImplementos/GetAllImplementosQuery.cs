using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Loans;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.Loans.GetImplementos;

public record GetAllImplementosQuery : IQuery<GetAllImplementosResponse>;

public record GetAllImplementosResponse(
    IReadOnlyList<ImplementoDto> Implementos
);

public record ImplementoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string Descripcion
);

internal sealed class GetAllImplementosQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetAllImplementosQuery, GetAllImplementosResponse> {

    public async Task<Result<GetAllImplementosResponse>> Handle(
        GetAllImplementosQuery request,
        CancellationToken cancellationToken
    ) {
        var repo = unitOfWork.Repository<Implemento>();
        var items = await repo.GetAllAsync( cancellationToken );

        var dtos = items
            .Select( i => new ImplementoDto( i.Id, i.Codigo, i.Nombre, i.Descripcion ) )
            .ToList();

        return Result.Success( new GetAllImplementosResponse( dtos ) );
    }
}

