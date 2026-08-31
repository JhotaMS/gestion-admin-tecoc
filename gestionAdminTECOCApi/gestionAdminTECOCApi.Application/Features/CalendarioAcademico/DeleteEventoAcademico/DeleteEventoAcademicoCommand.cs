using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.CalendarioAcademico;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.CalendarioAcademico.DeleteEventoAcademico;

public record DeleteEventoAcademicoCommand(
    Guid EventoAcademicoId
) : ICommand;

internal sealed class DeleteEventoAcademicoCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<DeleteEventoAcademicoCommand> {
    public async Task<Result> Handle(
        DeleteEventoAcademicoCommand request,
        CancellationToken cancellationToken
    ) {
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
            return Result.Failure( EventoAcademicoErrors.NotFound );

        repository.Delete( evento );
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
