using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Prestamos;

public static class PrestamoErrors {
    public static Error InvalidPageNumber => new(
        "Prestamo.InvalidPageNumber",
        "El número de página debe ser mayor o igual a 1"
    );

    public static Error InvalidPageSize( int maxPageSize ) => new(
        "Prestamo.InvalidPageSize",
        $"El tamaño de página debe estar entre 1 y {maxPageSize}"
    );
}
