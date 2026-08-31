using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.CalendarioAcademico;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.CalendarioAcademico.GetAllEventosAcademicos;

public record GetAllEventosAcademicosQuery : IQuery<GetAllEventosAcademicosResponse>;

public record GetAllEventosAcademicosResponse(
    IReadOnlyList<EventoAcademicoResponse> Eventos
);

internal sealed class GetAllEventosAcademicosQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetAllEventosAcademicosQuery, GetAllEventosAcademicosResponse> {
    public async Task<Result<GetAllEventosAcademicosResponse>> Handle(
        GetAllEventosAcademicosQuery request,
        CancellationToken cancellationToken
    ) {
        IReadOnlyList<EventoAcademico> eventos = await unitOfWork
            .Repository<EventoAcademico>()
            .GetAllAsync( cancellationToken );

        IReadOnlyList<EventoAcademicoResponse> response = eventos
            .OrderBy( evento => evento.FechaInicio )
            .Select( evento => new EventoAcademicoResponse(
                evento.Id,
                evento.Titulo,
                evento.Descripcion,
                evento.FechaInicio,
                evento.FechaFin,
                evento.Enabled
            ) )
            .ToList();

        return Result.Success( new GetAllEventosAcademicosResponse( response ) );
    }
}
