using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.DocumentTypes;

public class DocumentTypeEntity : Entity<Guid> {
    private DocumentTypeEntity(
        string code,
        string description
    ) : base( true ) {
        Id = Guid.NewGuid();
        Code = code;
        Description = description;
    }

    public string Code { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    public static DocumentTypeEntity Create(
        string code,
        string description
    ) => new( code, description );

    public void Update( string code, string description ) {
        Code = code;
        Description = description;
    }
}
