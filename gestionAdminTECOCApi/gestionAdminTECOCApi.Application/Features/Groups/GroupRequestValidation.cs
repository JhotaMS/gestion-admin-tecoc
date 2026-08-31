using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Groups;

namespace gestionAdminTECOCApi.Application.Features.Groups;

internal static class GroupRequestValidation {
    public static Error? Validate(
        string? name,
        string? code,
        int cupoTotal
    ) {
        if (string.IsNullOrWhiteSpace( name ))
            return GroupErrors.NameRequired;

        if (name.Trim().Length > Group.MaximumNameLength)
            return GroupErrors.NameTooLong;

        if (string.IsNullOrWhiteSpace( code ))
            return GroupErrors.CodeRequired;

        if (code.Trim().Length > Group.MaximumCodeLength)
            return GroupErrors.CodeTooLong;

        if (cupoTotal < 0)
            return GroupErrors.CupoTotalInvalido;

        return null;
    }
}
