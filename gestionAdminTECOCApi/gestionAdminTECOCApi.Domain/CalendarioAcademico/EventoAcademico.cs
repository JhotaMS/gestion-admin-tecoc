using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.CalendarioAcademico;

public class EventoAcademico : Entity<Guid> {
    public const int MaximumTituloLength = 150;
    public const int MaximumDescripcionLength = 500;

    private EventoAcademico(
        string titulo,
        string? descripcion,
        DateOnly fechaInicio,
        DateOnly? fechaFin
    ) : base( true ) {
        Id = Guid.NewGuid();
        Titulo = NormalizeTitulo( titulo );
        Descripcion = NormalizeDescripcion( descripcion );
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
    }

    public string Titulo { get; private set; } = default!;
    public string? Descripcion { get; private set; }
    public DateOnly FechaInicio { get; private set; }
    public DateOnly? FechaFin { get; private set; }

    public static EventoAcademico Create(
        string titulo,
        string? descripcion,
        DateOnly fechaInicio,
        DateOnly? fechaFin
    ) => new( titulo, descripcion, fechaInicio, fechaFin );

    public void Update(
        string titulo,
        string? descripcion,
        DateOnly fechaInicio,
        DateOnly? fechaFin
    ) {
        Titulo = NormalizeTitulo( titulo );
        Descripcion = NormalizeDescripcion( descripcion );
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
    }

    public static string NormalizeTitulo( string titulo ) => titulo.Trim();

    public static string? NormalizeDescripcion( string? descripcion ) =>
        string.IsNullOrWhiteSpace( descripcion ) ? null : descripcion.Trim();
}
