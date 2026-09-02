using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.CalendarioAcademico;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.CalendarioAcademico.CreateEventoAcademico;

internal sealed class CreateEventoAcademicoCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateEventoAcademicoCommand, EventoAcademicoResponse> {
    public async Task<Result<EventoAcademicoResponse>> Handle(
        CreateEventoAcademicoCommand request,
        CancellationToken cancellationToken
    ) {
        Error? validationError = EventoAcademicoRequestValidation.Validate(
            request.Titulo, request.Descripcion, request.FechaInicio, request.FechaFin
        );
        if (validationError is not null)
            return Result.Failure<EventoAcademicoResponse>( validationError );

        EventoAcademico evento = EventoAcademico.Create(
            request.Titulo, request.Descripcion, request.FechaInicio, request.FechaFin
        );

        await unitOfWork.Repository<EventoAcademico>().AddAsync( evento, cancellationToken );
        await unitOfWork.SaveChangesAsync();

        return Result.Success( ToResponse( evento ) );
    }

    private static EventoAcademicoResponse ToResponse( EventoAcademico evento ) => new(
        evento.Id,
        evento.Titulo,
        evento.Descripcion,
        evento.FechaInicio,
        evento.FechaFin,
        evento.Enabled
    );
}
