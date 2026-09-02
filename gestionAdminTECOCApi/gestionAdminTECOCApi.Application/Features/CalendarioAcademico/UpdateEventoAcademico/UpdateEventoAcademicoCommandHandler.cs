using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.CalendarioAcademico;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.CalendarioAcademico.UpdateEventoAcademico;

internal sealed class UpdateEventoAcademicoCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateEventoAcademicoCommand, EventoAcademicoResponse> {
    public async Task<Result<EventoAcademicoResponse>> Handle(
        UpdateEventoAcademicoCommand request,
        CancellationToken cancellationToken
    ) {
        Error? validationError = EventoAcademicoRequestValidation.Validate(
            request.Titulo, request.Descripcion, request.FechaInicio, request.FechaFin
        );
        if (validationError is not null)
            return Result.Failure<EventoAcademicoResponse>( validationError );

        var repository = unitOfWork.Repository<EventoAcademico>();
        IReadOnlyList<EventoAcademico> matches = await repository.GetAsync(
            evento => evento.Id == request.EventoAcademicoId,
            orderBy: null,
            includeString: null,
            disableTracking: false,
            cancellationToken: cancellationToken
        );

        EventoAcademico? evento = matches.SingleOrDefault();
        if (evento is null)
            return Result.Failure<EventoAcademicoResponse>( EventoAcademicoErrors.NotFound );

        evento.Update( request.Titulo, request.Descripcion, request.FechaInicio, request.FechaFin );
        await unitOfWork.SaveChangesAsync();

        return Result.Success( new EventoAcademicoResponse(
            evento.Id,
            evento.Titulo,
            evento.Descripcion,
            evento.FechaInicio,
            evento.FechaFin,
            evento.Enabled
        ) );
    }
}
