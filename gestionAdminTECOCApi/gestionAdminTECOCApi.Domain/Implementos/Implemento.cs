using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Implementos;

public class Implemento : Entity<Guid> {
    private Implemento(
        string codigo,
        string nombre,
        string descripcion,
        int cantidadTotal,
        int cantidadDisponible,
        string estado,
        bool enabled
    ) : base( enabled ) {
        Id = Guid.NewGuid();
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        CantidadTotal = cantidadTotal;
        CantidadDisponible = cantidadDisponible;
        Estado = estado;
    }

    public string Codigo { get; private set; } = default!;
    public string Nombre { get; private set; } = default!;
    public string Descripcion { get; private set; } = default!;
    public int CantidadTotal { get; private set; }
    public int CantidadDisponible { get; private set; }
    public string Estado { get; private set; } = default!;

    public static Implemento Create(
        string codigo,
        string nombre,
        string descripcion,
        int cantidadTotal,
        int cantidadDisponible,
        string estado,
        bool activo
    ) => new(
        codigo,
        nombre,
        descripcion,
        cantidadTotal,
        cantidadDisponible,
        estado,
        activo
    );
}
