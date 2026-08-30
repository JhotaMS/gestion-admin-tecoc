using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Loans;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Loans.GetAllImplementosPrestados;

public record GetAllImplementosPrestadosQuery : IQuery<GetAllImplementosPrestadosResponse>;

public record GetAllImplementosPrestadosResponse(
    IReadOnlyList<ImplementoPrestadoDto> ImplementosPrestados
);

public record ImplementoPrestadoDto(
    Guid Id,
    Guid UserId,
    string RequesterName,
    Guid ImplementoId,
    string ItemName,
    string ItemCode,
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

        // ImplementoPrestado solo guarda los ids de usuario e implemento (no hay FK real
        // configurada), así que el nombre/código se completa aquí mismo trayendo los
        // catálogos completos y cruzándolos en memoria: son tablas pequeñas y evita ir
        // a la base de datos una vez por cada préstamo.
        var implementosById = (await unitOfWork.Repository<Implemento>().GetAllAsync( cancellationToken ))
            .ToDictionary( implemento => implemento.Id );

        var usersById = (await unitOfWork.Repository<User>().GetAllAsync( cancellationToken ))
            .ToDictionary( user => user.Id );

        var dtos = items
            .Select( x => {
                implementosById.TryGetValue( x.ImplementoId, out var implemento );
                usersById.TryGetValue( x.UserId, out var user );

                return new ImplementoPrestadoDto(
                    x.Id,
                    x.UserId,
                    user?.FullName ?? "Usuario no encontrado",
                    x.ImplementoId,
                    implemento?.Nombre ?? "Implemento no encontrado",
                    implemento?.Codigo ?? "-",
                    x.TipoRevisionId,
                    x.EstadoTipo.ToString(),
                    x.FechaInicio,
                    x.FechaFin,
                    x.Observacion
                );
            } )
            .ToList();

        return Result.Success( new GetAllImplementosPrestadosResponse( dtos ) );
    }
}
