using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.ProgramasAcademicos;

public class ProgramaAcademico : Entity<Guid> {
    public const int MaximumNameLength = 150;
    public const int MaximumCodeLength = 30;

    private ProgramaAcademico(
        string name,
        string code
    ) : base( true ) {
        Id = Guid.NewGuid();
        Name = NormalizeName( name );
        Code = NormalizeCode( code );
    }

    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;

    public static ProgramaAcademico Create(
        string name,
        string code
    ) => new( name, code );

    public void Update(
        string name,
        string code
    ) {
        Name = NormalizeName( name );
        Code = NormalizeCode( code );
    }

    public static string NormalizeName( string name ) => name.Trim();

    public static string NormalizeCode( string code ) => code.Trim().ToUpperInvariant();
}
