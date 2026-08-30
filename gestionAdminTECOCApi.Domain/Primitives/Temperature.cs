namespace gestionAdminTECOCApi.Domain.Primitives;

public record Temperature {

    private const int TEMPERATURE_BASE = 32;
    private const decimal INDICE_RAMDON = 0.5556m;

    public Temperature( int value ) {
        Value = value;
    }
    public int Value { get; private set; }

    public static Temperature Generated( int temperatureC ) =>
        new( TEMPERATURE_BASE + (int)(temperatureC / INDICE_RAMDON) );
}
