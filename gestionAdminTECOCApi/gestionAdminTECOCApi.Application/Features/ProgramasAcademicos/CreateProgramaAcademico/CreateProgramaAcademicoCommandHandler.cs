using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.ProgramasAcademicos;

namespace gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.CreateProgramaAcademico;

internal sealed class CreateProgramaAcademicoCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateProgramaAcademicoCommand, ProgramaAcademicoResponse> {
    public async Task<Result<ProgramaAcademicoResponse>> Handle(
        CreateProgramaAcademicoCommand request,
        CancellationToken cancellationToken
    ) {
        Error? validationError = ProgramaAcademicoRequestValidation.Validate( request.Name, request.Code );
        if (validationError is not null)
            return Result.Failure<ProgramaAcademicoResponse>( validationError );

        string normalizedCode = ProgramaAcademico.NormalizeCode( request.Code );
        var repository = unitOfWork.Repository<ProgramaAcademico>();
        IReadOnlyList<ProgramaAcademico> existing = await repository.GetAsync(
            programa => programa.Code == normalizedCode,
            cancellationToken
        );

        if (existing.Any())
            return Result.Failure<ProgramaAcademicoResponse>( ProgramaAcademicoErrors.DuplicateCode );

        ProgramaAcademico programa = ProgramaAcademico.Create( request.Name, request.Code );
        await repository.AddAsync( programa, cancellationToken );
        await unitOfWork.SaveChangesAsync();

        return Result.Success( ToResponse( programa ) );
    }

    private static ProgramaAcademicoResponse ToResponse( ProgramaAcademico programa ) => new(
        programa.Id,
        programa.Name,
        programa.Code,
        programa.Enabled
    );
}
