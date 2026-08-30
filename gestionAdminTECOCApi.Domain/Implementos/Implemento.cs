using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Implementos;

public class Implemento : Entity<Guid> {
    private Implemento(
        string nombre,
        string codigo,
        string? descripcion,
        int cantidadTotal,
        int cantidadDisponible,
        string estado,
        bool enabled
    ) : base( enabled ) {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Codigo = codigo;
        Descripcion = descripcion;
        CantidadTotal = cantidadTotal;
        CantidadDisponible = cantidadDisponible;
        Estado = estado;
    }

    public string Nombre { get; private set; }
    public string Codigo { get; private set; }
    public string? Descripcion { get; private set; }
    public int CantidadTotal { get; private set; }
    public int CantidadDisponible { get; private set; }
    public string Estado { get; private set; }

    public static Implemento Create(
        string nombre,
        string codigo,
        string? descripcion,
        int cantidadTotal,
        int cantidadDisponible,
        string estado,
        bool enabled = true
    ) {
        ArgumentException.ThrowIfNullOrWhiteSpace( nombre );
        ArgumentException.ThrowIfNullOrWhiteSpace( codigo );
        ArgumentException.ThrowIfNullOrWhiteSpace( estado );

        if (cantidadTotal < 0) {
            throw new ArgumentOutOfRangeException( nameof( cantidadTotal ) );
        }

        if (cantidadDisponible < 0 || cantidadDisponible > cantidadTotal) {
            throw new ArgumentOutOfRangeException( nameof( cantidadDisponible ) );
        }

        return new(
            nombre,
            codigo,
            descripcion,
            cantidadTotal,
            cantidadDisponible,
            estado,
            enabled
        );
    }

    public static string[] Estados() =>
        [
            ImplementoEstados.Disponible,
            ImplementoEstados.Prestado,
            ImplementoEstados.Mantenimiento
        ];
}
