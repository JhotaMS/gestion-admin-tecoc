using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Loans;

public class TipoRevision : Entity<int> {
    private TipoRevision(
        int id,
        string nombre,
        string descripcion
    ) : base( true ) {
        Id = id;
        Nombre = nombre;
        Descripcion = descripcion;
    }

    private TipoRevision() : base( true ) { }

    public string Nombre { get; private set; } = default!;
    public string Descripcion { get; private set; } = default!;

    public static TipoRevision Create(
        int id,
        string nombre,
        string descripcion
    ) => new( id, nombre, descripcion );
}

