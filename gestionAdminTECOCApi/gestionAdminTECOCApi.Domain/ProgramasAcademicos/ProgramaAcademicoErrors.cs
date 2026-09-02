using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.ProgramasAcademicos;

public static class ProgramaAcademicoErrors {
    public static readonly Error NameRequired = new(
        "ProgramaAcademico.NameRequired",
        "El nombre del programa académico es obligatorio"
    );

    public static readonly Error NameTooLong = new(
        "ProgramaAcademico.NameTooLong",
        $"El nombre del programa académico no puede superar los {ProgramaAcademico.MaximumNameLength} caracteres"
    );

    public static readonly Error CodeRequired = new(
        "ProgramaAcademico.CodeRequired",
        "El código del programa académico es obligatorio"
    );

    public static readonly Error CodeTooLong = new(
        "ProgramaAcademico.CodeTooLong",
        $"El código del programa académico no puede superar los {ProgramaAcademico.MaximumCodeLength} caracteres"
    );

    public static readonly Error DuplicateCode = new(
        "ProgramaAcademico.DuplicateCode",
        "Ya existe un programa académico con ese código"
    );

    public static readonly Error NotFound = new(
        "ProgramaAcademico.NotFound",
        "No se encontró el programa académico solicitado"
    );
}
