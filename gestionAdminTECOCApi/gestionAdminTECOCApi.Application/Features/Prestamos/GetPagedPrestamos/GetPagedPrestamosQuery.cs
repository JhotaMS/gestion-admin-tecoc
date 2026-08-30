using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Loans;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Prestamos;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Prestamos.GetPagedPrestamos;

// PageNumber arranca en 1 (no en 0). PageSize es cuántos préstamos trae cada página.
public record GetPagedPrestamosQuery( int PageNumber = 1, int PageSize = 10 ) : IQuery<GetPagedPrestamosResponse>;

// Por cada llave foránea se trae el Id crudo Y el nombre ya resuelto (RequesterName,
// ImplementoNombre, TipoRevisionNombre), para que el frontend no tenga que hacer
// consultas aparte solo para mostrar un nombre legible.
public record PrestamoDto(
    Guid Id,
    Guid UuserId,
    string RequesterName,
    Guid ImplementoId,
    string ImplementoNombre,
    int TipoRevisionId,
    string TipoRevisionNombre,
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
    private const string NombreNoEncontrado = "(no encontrado)";

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

        // Se resuelven los 3 nombres con UNA sola consulta batch por cada tabla relacionada
        // (usuarios, implementos, tipos de revisión), usando los Id distintos de esta página,
        // en vez de una consulta por cada préstamo (que sería N+1 tres veces).
        var userIds = items.Select( p => p.UuserId ).Distinct().ToList();
        var users = await unitOfWork.Repository<User>().GetAsync(
            u => userIds.Contains( u.Id ), cancellationToken: cancellationToken );
        var userNamesById = users.ToDictionary( u => u.Id, u => u.FullName );

        var implementoIds = items.Select( p => p.ImplementoId ).Distinct().ToList();
        var implementos = await unitOfWork.Repository<Implemento>().GetAsync(
            i => implementoIds.Contains( i.Id ), cancellationToken: cancellationToken );
        var implementoNamesById = implementos.ToDictionary( i => i.Id, i => i.Nombre );

        var tipoRevisionIds = items.Select( p => p.TipoRevisionId ).Distinct().ToList();
        var tiposRevision = await unitOfWork.Repository<TipoRevision>().GetAsync(
            t => tipoRevisionIds.Contains( t.Id ), cancellationToken: cancellationToken );
        var tipoRevisionNamesById = tiposRevision.ToDictionary( t => t.Id, t => t.Nombre );

        var dtos = items
            .Select( p => new PrestamoDto(
                p.Id,
                p.UuserId,
                userNamesById.GetValueOrDefault( p.UuserId, NombreNoEncontrado ),
                p.ImplementoId,
                implementoNamesById.GetValueOrDefault( p.ImplementoId, NombreNoEncontrado ),
                p.TipoRevisionId,
                tipoRevisionNamesById.GetValueOrDefault( p.TipoRevisionId, NombreNoEncontrado ),
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
