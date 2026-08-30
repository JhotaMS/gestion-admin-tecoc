using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Prestamos;

namespace gestionAdminTECOCApi.Application.Features.Prestamos.GetPagedPrestamos;

// PageNumber arranca en 1 (no en 0). PageSize es cuántos préstamos trae cada página.
public record GetPagedPrestamosQuery( int PageNumber = 1, int PageSize = 10 ) : IQuery<GetPagedPrestamosResponse>;

public record PrestamoDto(
    Guid Id,
    Guid UuserId,
    Guid ImplementoId,
    Guid TipoRevisionId,
    string EstadoTipo,
    DateTime Inicio,
    DateTime Fin,
    string Observacion
);

// TotalCount/TotalPages le permiten al frontend armar los controles de paginación
// (ej. "página 2 de 5") sin tener que descargar todos los préstamos para contarlos.
public record GetPagedPrestamosResponse(
    IReadOnlyList<PrestamoDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
);

internal sealed class GetPagedPrestamosQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetPagedPrestamosQuery, GetPagedPrestamosResponse> {

    private const int MaxPageSize = 100;

    public async Task<Result<GetPagedPrestamosResponse>> Handle(
        GetPagedPrestamosQuery request,
        CancellationToken cancellationToken
    ) {
        if (request.PageNumber < 1)
            return Result.Failure<GetPagedPrestamosResponse>( PrestamoErrors.InvalidPageNumber );

        if (request.PageSize < 1 || request.PageSize > MaxPageSize)
            return Result.Failure<GetPagedPrestamosResponse>( PrestamoErrors.InvalidPageSize( MaxPageSize ) );

        var repo = unitOfWork.Repository<Prestamo>();

        // GetPagedAsync hace el conteo total y el corte (skip/take) dentro de la consulta SQL,
        // por lo que nunca se trae la tabla completa a memoria solo para paginarla.
        var (items, totalCount) = await repo.GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            orderBy: query => query.OrderByDescending( p => p.Inicio ),
            cancellationToken: cancellationToken
        );

        var dtos = items
            .Select( p => new PrestamoDto(
                p.Id,
                p.UuserId,
                p.ImplementoId,
                p.TipoRevisionId,
                p.EstadoTipo,
                p.Inicio,
                p.Fin,
                p.Observacion
            ) )
            .ToList();

        // Si no hay préstamos, TotalCount es 0 y TotalPages queda en 0 (no en 1), para que el
        // frontend pueda distinguir "tabla vacía" de "página fuera de rango".
        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling( totalCount / (double)request.PageSize );

        return Result.Success( new GetPagedPrestamosResponse(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount,
            totalPages
        ) );
    }
}
