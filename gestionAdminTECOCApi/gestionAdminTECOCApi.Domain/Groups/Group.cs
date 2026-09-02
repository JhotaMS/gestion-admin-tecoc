using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Groups;

public class Group : Entity<Guid> {
    public const int MaximumNameLength = 100;
    public const int MaximumCodeLength = 30;

    private Group(
        string name,
        string code,
        int cupoTotal
    ) : base( true ) {
        Id = Guid.NewGuid();
        Name = NormalizeName( name );
        Code = NormalizeCode( code );
        CupoTotal = cupoTotal;
    }

    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public int CupoTotal { get; private set; }

    public static Group Create(
        string name,
        string code,
        int cupoTotal
    ) => new( name, code, cupoTotal );

    public void Update(
        string name,
        string code,
        int cupoTotal
    ) {
        Name = NormalizeName( name );
        Code = NormalizeCode( code );
        CupoTotal = cupoTotal;
    }

    public static string NormalizeName( string name ) => name.Trim();

    public static string NormalizeCode( string code ) => code.Trim().ToUpperInvariant();
}
