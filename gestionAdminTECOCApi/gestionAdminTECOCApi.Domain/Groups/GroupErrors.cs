using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Groups;

public static class GroupErrors {
    public static readonly Error NameRequired = new(
        "Group.NameRequired",
        "El nombre del grupo es obligatorio"
    );

    public static readonly Error NameTooLong = new(
        "Group.NameTooLong",
        $"El nombre del grupo no puede superar los {Group.MaximumNameLength} caracteres"
    );

    public static readonly Error CodeRequired = new(
        "Group.CodeRequired",
        "El código del grupo es obligatorio"
    );

    public static readonly Error CodeTooLong = new(
        "Group.CodeTooLong",
        $"El código del grupo no puede superar los {Group.MaximumCodeLength} caracteres"
    );

    public static readonly Error DuplicateCode = new(
        "Group.DuplicateCode",
        "Ya existe un grupo con ese código"
    );

    public static readonly Error NotFound = new(
        "Group.NotFound",
        "No se encontró el grupo solicitado"
    );
}
