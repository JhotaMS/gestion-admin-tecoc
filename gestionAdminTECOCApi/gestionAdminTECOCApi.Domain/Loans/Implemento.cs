using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Loans;

public class Implemento : Entity<Guid> {
    private Implemento(
        Guid id,
        string codigo,
        string nombre,
        string descripcion,
        int cantidadTotal,
        int cantidadDisponible,
        string estado,
        bool activo
    ) : base( activo ) {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        CantidadTotal = cantidadTotal;
        CantidadDisponible = cantidadDisponible;
        Estado = estado;
    }

    private Implemento() : base( true ) { }

    public string Codigo { get; private set; } = default!;
    public string Nombre { get; private set; } = default!;
    public string Descripcion { get; private set; } = default!;
    public int CantidadTotal { get; private set; }
    public int CantidadDisponible { get; private set; }
    public string Estado { get; private set; } = string.Empty;

    public static Implemento Create(
        string codigo,
        string nombre,
        string descripcion,
        Guid? id = null,
        int cantidadTotal = 0,
        int cantidadDisponible = 0,
        string estado = "",
        bool activo = true
    ) => new(
        id ?? Guid.NewGuid(),
        codigo,
        nombre,
        descripcion,
        cantidadTotal,
        cantidadDisponible,
        estado,
        activo
    );
}

