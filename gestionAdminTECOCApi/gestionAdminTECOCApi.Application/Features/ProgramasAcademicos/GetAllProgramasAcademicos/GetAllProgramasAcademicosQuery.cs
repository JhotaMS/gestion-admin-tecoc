using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.ProgramasAcademicos;

namespace gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.GetAllProgramasAcademicos;

public record GetAllProgramasAcademicosQuery : IQuery<GetAllProgramasAcademicosResponse>;

public record GetAllProgramasAcademicosResponse(
    IReadOnlyList<ProgramaAcademicoResponse> ProgramasAcademicos
);

internal sealed class GetAllProgramasAcademicosQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetAllProgramasAcademicosQuery, GetAllProgramasAcademicosResponse> {
    public async Task<Result<GetAllProgramasAcademicosResponse>> Handle(
        GetAllProgramasAcademicosQuery request,
        CancellationToken cancellationToken
    ) {
        IReadOnlyList<ProgramaAcademico> programas = await unitOfWork
            .Repository<ProgramaAcademico>()
            .GetAllAsync( cancellationToken );

        IReadOnlyList<ProgramaAcademicoResponse> response = programas
            .OrderBy( programa => programa.Name )
            .Select( programa => new ProgramaAcademicoResponse(
                programa.Id,
                programa.Name,
                programa.Code,
                programa.Enabled
            ) )
            .ToList();

        return Result.Success( new GetAllProgramasAcademicosResponse( response ) );
    }
}
