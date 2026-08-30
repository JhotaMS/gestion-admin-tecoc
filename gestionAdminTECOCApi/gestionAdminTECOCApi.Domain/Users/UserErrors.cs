using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Users;

public static Error NotFound( Guid id ) => new(
    "User.NotFound",
    $"No se encontró un usuario con el identificador '{id}'"
);

public static Error AlreadyDisabled( Guid id ) => new(
    "User.AlreadyDisabled",
    $"El usuario con identificador '{id}' ya se encuentra inactivo"
);