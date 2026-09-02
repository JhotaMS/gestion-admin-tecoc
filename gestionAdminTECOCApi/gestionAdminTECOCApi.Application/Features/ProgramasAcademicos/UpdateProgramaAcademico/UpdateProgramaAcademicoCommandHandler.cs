using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.ProgramasAcademicos;

namespace gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.UpdateProgramaAcademico;

internal sealed class UpdateProgramaAcademicoCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateProgramaAcademicoCommand, ProgramaAcademicoResponse> {
    public async Task<Result<ProgramaAcademicoResponse>> Handle(
        UpdateProgramaAcademicoCommand request,
        CancellationToken cancellationToken
    ) {
        Error? validationError = ProgramaAcademicoRequestValidation.Validate( request.Name, request.Code );
        if (validationError is not null)
            return Result.Failure<ProgramaAcademicoResponse>( validationError );

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
            return Result.Failure<ProgramaAcademicoResponse>( ProgramaAcademicoErrors.NotFound );

        string normalizedCode = ProgramaAcademico.NormalizeCode( request.Code );
        IReadOnlyList<ProgramaAcademico> duplicateCodes = await repository.GetAsync(
            candidate => candidate.Id != request.ProgramaAcademicoId && candidate.Code == normalizedCode,
            cancellationToken
        );

        if (duplicateCodes.Any())
            return Result.Failure<ProgramaAcademicoResponse>( ProgramaAcademicoErrors.DuplicateCode );

        programa.Update( request.Name, request.Code );
        await unitOfWork.SaveChangesAsync();

        return Result.Success( new ProgramaAcademicoResponse(
            programa.Id,
            programa.Name,
            programa.Code,
            programa.Enabled
        ) );
    }
}
