using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Loans;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Prestamos;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Prestamos.GetPrestamoById;

public record GetPrestamoByIdQuery( Guid Id ) : IQuery<GetPrestamoByIdResponse>;

// Trae, además de lo que ya expone el listado paginado, los datos que pide la vista de
// detalle: correo y documento del usuario, y el estado propio del implemento (no el del
// préstamo, que ya viene en EstadoTipo).
public record GetPrestamoByIdResponse(
    Guid Id,
    Guid UuserId,
    string RequesterName,
    string RequesterEmail,
    string RequesterDocumentType,
    string RequesterDocumentNumber,
    Guid ImplementoId,
    string ImplementoNombre,
    string ImplementoEstado,
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
            user?.Email ?? NombreNoEncontrado,
            user is null ? NombreNoEncontrado : DocumentTypeCodes.ToDescription( user.DocumentType ),
            user?.DocumentNumber ?? NombreNoEncontrado,
            prestamo.ImplementoId,
            implemento?.Nombre ?? NombreNoEncontrado,
            implemento?.Estado ?? NombreNoEncontrado,
            prestamo.TipoRevisionId,
            tipoRevision?.Nombre ?? NombreNoEncontrado,
            prestamo.EstadoTipo,
            prestamo.Inicio,
            prestamo.Fin,
            prestamo.Observacion
        ) );
    }
}
