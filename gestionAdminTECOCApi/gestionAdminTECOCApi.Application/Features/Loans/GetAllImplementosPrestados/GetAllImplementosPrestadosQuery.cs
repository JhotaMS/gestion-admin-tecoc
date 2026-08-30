using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Loans;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.Loans.GetAllImplementosPrestados;

public record GetAllImplementosPrestadosQuery : IQuery<GetAllImplementosPrestadosResponse>;

public record GetAllImplementosPrestadosResponse(
    IReadOnlyList<ImplementoPrestadoDto> ImplementosPrestados
);

public record ImplementoPrestadoDto(
    Guid Id,
    Guid UserId,
    Guid ImplementoId,
    int TipoRevisionId,
    string EstadoTipo,
    DateTime FechaInicio,
    DateTime FechaFin,
    string? Observacion
);

internal sealed class GetAllImplementosPrestadosQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetAllImplementosPrestadosQuery, GetAllImplementosPrestadosResponse> {

    public async Task<Result<GetAllImplementosPrestadosResponse>> Handle(
        GetAllImplementosPrestadosQuery request,
        CancellationToken cancellationToken
    ) {
        var repo = unitOfWork.Repository<ImplementoPrestado>();
        var items = await repo.GetAllAsync( cancellationToken );

        var dtos = items
            .Select( x => new ImplementoPrestadoDto(
                x.Id,
                x.UserId,
                x.ImplementoId,
                x.TipoRevisionId,
                x.EstadoTipo.ToString(),
                x.FechaInicio,
                x.FechaFin,
                x.Observacion
            ) )
            .ToList();

        return Result.Success( new GetAllImplementosPrestadosResponse( dtos ) );
    }
}

