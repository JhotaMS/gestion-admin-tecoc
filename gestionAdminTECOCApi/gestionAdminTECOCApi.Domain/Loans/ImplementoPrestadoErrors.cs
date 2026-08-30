using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Loans;

public static class ImplementoPrestadoErrors {
    public static readonly Error UserNotFound = new(
        "ImplementoPrestado.UserNotFound",
        "El usuario especificado no existe en el sistema."
    );

    public static readonly Error ImplementoNotFound = new(
        "ImplementoPrestado.ImplementoNotFound",
        "El implemento especificado no existe en el sistema."
    );

    public static readonly Error TipoRevisionNotFound = new(
        "ImplementoPrestado.TipoRevisionNotFound",
        "El tipo de revisión especificado no es válido."
    );

    public static readonly Error InvalidDateRange = new(
        "ImplementoPrestado.InvalidDateRange",
        "La fecha de fin debe ser igual o posterior a la fecha de inicio."
    );

    public static readonly Error NotFound = new(
        "ImplementoPrestado.NotFound",
        "El registro del préstamo de implemento no fue encontrado."
    );
}

