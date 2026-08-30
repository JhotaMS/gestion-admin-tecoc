using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Loans;

public class ImplementoPrestado : Entity<Guid> {
    private ImplementoPrestado(
        Guid id,
        Guid userId,
        Guid implementoId,
        int tipoRevisionId,
        EstadoTipoImplemento estadoTipo,
        DateTime fechaInicio,
        DateTime fechaFin,
        string? observacion
    ) : base( true ) {
        Id = id;
        UserId = userId;
        ImplementoId = implementoId;
        TipoRevisionId = tipoRevisionId;
        EstadoTipo = estadoTipo;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        Observacion = observacion;
    }

    private ImplementoPrestado() : base( true ) { }

    public Guid UserId { get; private set; }
    public Guid ImplementoId { get; private set; }
    public int TipoRevisionId { get; private set; }
    public EstadoTipoImplemento EstadoTipo { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaFin { get; private set; }
    public string? Observacion { get; private set; }

    public static ImplementoPrestado Create(
        Guid userId,
        Guid implementoId,
        int tipoRevisionId,
        EstadoTipoImplemento estadoTipo,
        DateTime fechaInicio,
        DateTime fechaFin,
        string? observacion = null,
        Guid? id = null
    ) => new(
        id ?? Guid.NewGuid(),
        userId,
        implementoId,
        tipoRevisionId,
        estadoTipo,
        fechaInicio,
        fechaFin,
        observacion
    );

    public void ActualizarEstado(
        int tipoRevisionId,
        EstadoTipoImplemento estadoTipo,
        DateTime fechaInicio,
        DateTime fechaFin,
        string? observacion
    ) {
        TipoRevisionId = tipoRevisionId;
        EstadoTipo = estadoTipo;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        Observacion = observacion;
    }
}

