using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.ProgramasAcademicos;

namespace gestionAdminTECOCApi.Application.Features.ProgramasAcademicos;

internal static class ProgramaAcademicoRequestValidation {
    public static Error? Validate(
        string? name,
        string? code
    ) {
        if (string.IsNullOrWhiteSpace( name ))
            return ProgramaAcademicoErrors.NameRequired;

        if (name.Trim().Length > ProgramaAcademico.MaximumNameLength)
            return ProgramaAcademicoErrors.NameTooLong;

        if (string.IsNullOrWhiteSpace( code ))
            return ProgramaAcademicoErrors.CodeRequired;

        if (code.Trim().Length > ProgramaAcademico.MaximumCodeLength)
            return ProgramaAcademicoErrors.CodeTooLong;

        return null;
    }
}
