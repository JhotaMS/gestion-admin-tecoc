using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.CreateProgramaAcademico;

public record CreateProgramaAcademicoCommand(
    string Name,
    string Code
) : ICommand<ProgramaAcademicoResponse>;
