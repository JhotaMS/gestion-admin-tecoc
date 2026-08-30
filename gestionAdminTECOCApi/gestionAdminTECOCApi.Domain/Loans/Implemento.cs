using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Loans;

public class Implemento : Entity<Guid> {
    private Implemento(
        Guid id,
        string codigo,
        string nombre,
        string descripcion
    ) : base( true ) {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
    }

    private Implemento() : base( true ) { }

    public string Codigo { get; private set; } = default!;
    public string Nombre { get; private set; } = default!;
    public string Descripcion { get; private set; } = default!;

    public static Implemento Create(
        string codigo,
        string nombre,
        string descripcion,
        Guid? id = null
    ) => new( id ?? Guid.NewGuid(), codigo, nombre, descripcion );
}

