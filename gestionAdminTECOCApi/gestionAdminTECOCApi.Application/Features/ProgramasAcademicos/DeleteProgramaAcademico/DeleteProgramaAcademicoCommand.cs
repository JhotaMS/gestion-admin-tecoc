using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.ProgramasAcademicos;

namespace gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.DeleteProgramaAcademico;

public record DeleteProgramaAcademicoCommand(
    Guid ProgramaAcademicoId
) : ICommand;

internal sealed class DeleteProgramaAcademicoCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<DeleteProgramaAcademicoCommand> {
    public async Task<Result> Handle(
        DeleteProgramaAcademicoCommand request,
        CancellationToken cancellationToken
    ) {
        var repository = unitOfWork.Repository<ProgramaAcademico>();
        IReadOnlyList<ProgramaAcademico> matches = await repository.GetAsync(
            programa => programa.Id == request.ProgramaAcademicoId,
            orderBy: null,
            includeString: null,
            disableTracking: false,
            cancellationToken: cancellationToken
        );

        ProgramaAcademico? programa = matches.SingleOrDefault();
        if (programa is null)
            return Result.Failure( ProgramaAcademicoErrors.NotFound );

        repository.Delete( programa );
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
