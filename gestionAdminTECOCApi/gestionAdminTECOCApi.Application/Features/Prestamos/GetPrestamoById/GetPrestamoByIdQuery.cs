using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Loans;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Prestamos;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Prestamos.GetPrestamoById;

public record GetPrestamoByIdQuery( Guid Id ) : IQuery<GetPrestamoByIdResponse>;

// Mismo shape que PrestamoDto en GetPagedPrestamos (Id/nombre resuelto por cada FK), para que
// el frontend reutilice el mismo modelo tanto en la tabla paginada como en el detalle.
public record GetPrestamoByIdResponse(
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

internal sealed class GetPrestamoByIdQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetPrestamoByIdQuery, GetPrestamoByIdResponse> {

    private const string NombreNoEncontrado = "(no encontrado)";

    public async Task<Result<GetPrestamoByIdResponse>> Handle(
        GetPrestamoByIdQuery request,
        CancellationToken cancellationToken
    ) {
        var prestamo = await unitOfWork.Repository<Prestamo>().GetByAsync(
            p => p.Id == request.Id, cancellationToken: cancellationToken );

        if (prestamo is null)
            return Result.Failure<GetPrestamoByIdResponse>( PrestamoErrors.NotFound( request.Id ) );

        var user = await unitOfWork.Repository<User>().GetByAsync(
            u => u.Id == prestamo.UuserId, cancellationToken: cancellationToken );
        var implemento = await unitOfWork.Repository<Implemento>().GetByAsync(
            i => i.Id == prestamo.ImplementoId, cancellationToken: cancellationToken );
        var tipoRevision = await unitOfWork.Repository<TipoRevision>().GetByAsync(
            t => t.Id == prestamo.TipoRevisionId, cancellationToken: cancellationToken );

        return Result.Success( new GetPrestamoByIdResponse(
            prestamo.Id,
            prestamo.UuserId,
            user?.FullName ?? NombreNoEncontrado,
            prestamo.ImplementoId,
            implemento?.Nombre ?? NombreNoEncontrado,
            prestamo.TipoRevisionId,
            tipoRevision?.Nombre ?? NombreNoEncontrado,
            prestamo.EstadoTipo,
            prestamo.Inicio,
            prestamo.Fin,
            prestamo.Observacion
        ) );
    }
}
