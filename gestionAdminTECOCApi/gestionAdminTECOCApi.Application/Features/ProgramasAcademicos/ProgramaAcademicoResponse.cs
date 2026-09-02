namespace gestionAdminTECOCApi.Application.Features.ProgramasAcademicos;

public record ProgramaAcademicoResponse(
    Guid Id,
    string Name,
    string Code,
    bool Enabled
);
