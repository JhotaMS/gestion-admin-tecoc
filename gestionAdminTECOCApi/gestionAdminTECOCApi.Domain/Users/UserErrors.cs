using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Users;

public static class UserErrors {
    public static Error DocumentTypeNotAllowed( string? documentType ) => new(
        "User.DocumentTypeNotAllowed",
        $"El tipo de documento '{documentType}' no es un valor válido configurado en el sistema. Los valores permitidos son: {string.Join( ", ", DocumentTypeCodes.AllowedCodes )}"
    );

    public static Error DocumentAlreadyRegistered( string documentType, string documentNumber ) => new(
        "User.DocumentAlreadyRegistered",
        $"Ya existe un registro con el tipo de documento '{documentType}' y el número de documento '{documentNumber}'"
    );

    public static Error NotFound( Guid userId ) => new(
        "User.NotFound",
        $"No se encontró el usuario con id '{userId}'"
    );
}
