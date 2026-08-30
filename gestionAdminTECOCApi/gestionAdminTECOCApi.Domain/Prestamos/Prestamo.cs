using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Prestamos;

public class Prestamo : Entity<Guid> {
    public Guid UuserId { get; set; }
    public Guid ImplementoId { get; set; }
    public Guid TipoRevisionId { get; set; }
    public string EstadoTipo { get; set; } = string.Empty;
    public DateTime Inicio { get; set; }
    public DateTime Fin { get; set; }
    public string Observacion { get; set; } = string.Empty;
}
