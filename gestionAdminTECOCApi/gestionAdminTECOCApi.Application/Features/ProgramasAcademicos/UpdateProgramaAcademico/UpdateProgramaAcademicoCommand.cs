using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.UpdateProgramaAcademico;

public record UpdateProgramaAcademicoCommand(
    Guid ProgramaAcademicoId,
    string Name,
    string Code
) : ICommand<ProgramaAcademicoResponse>;
