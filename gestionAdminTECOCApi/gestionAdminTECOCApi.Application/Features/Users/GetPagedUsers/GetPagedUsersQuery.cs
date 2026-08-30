using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Users.GetPagedUsers;

// PageNumber arranca en 1 (no en 0). PageSize es cuántos usuarios trae cada página.
public record GetPagedUsersQuery( int PageNumber = 1, int PageSize = 10 ) : IQuery<GetPagedUsersResponse>;

public record PagedUserDto(
    Guid Id,
    string FullName,
    string UserName,
    string Email,
    string DocumentType,
    string DocumentNumber
);

// TotalCount/TotalPages le permiten al frontend armar los controles de paginación
// (ej. "página 2 de 3") sin tener que descargar todos los usuarios para contarlos.
public record GetPagedUsersResponse(
    IReadOnlyList<PagedUserDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
);

internal sealed class GetPagedUsersQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetPagedUsersQuery, GetPagedUsersResponse> {

    private const int MaxPageSize = 100;

    public async Task<Result<GetPagedUsersResponse>> Handle(
        GetPagedUsersQuery request,
        CancellationToken cancellationToken
    ) {
        if (request.PageNumber < 1)
            return Result.Failure<GetPagedUsersResponse>( UserErrors.InvalidPageNumber );

        if (request.PageSize < 1 || request.PageSize > MaxPageSize)
            return Result.Failure<GetPagedUsersResponse>( UserErrors.InvalidPageSize( MaxPageSize ) );

        var repo = unitOfWork.Repository<User>();

        // GetPagedAsync hace el conteo total y el corte (skip/take) dentro de la consulta SQL,
        // por lo que nunca se trae la tabla completa a memoria solo para paginarla.
        var (items, totalCount) = await repo.GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            orderBy: query => query.OrderBy( u => u.FullName ),
            cancellationToken: cancellationToken
        );

        var dtos = items
            .Select( u => new PagedUserDto(
                u.Id,
                u.FullName,
                u.UserName,
                u.Email,
                DocumentTypeCodes.ToDescription( u.DocumentType ),
                u.DocumentNumber
            ) )
            .ToList();

        // Si no hay usuarios, TotalCount es 0 y TotalPages queda en 0 (no en 1), para que el
        // frontend pueda distinguir "tabla vacía" de "página fuera de rango".
        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling( totalCount / (double)request.PageSize );

        return Result.Success( new GetPagedUsersResponse(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount,
            totalPages
        ) );
    }
}
