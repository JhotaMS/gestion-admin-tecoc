using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Groups;

public class Group : Entity<Guid> {
    private Group(
        string name,
        string code
    ) : base( true ) {
        Id = Guid.NewGuid();
        Name = name;
        Code = code;
    }

    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;

    public static Group Create(
        string name,
        string code
    ) => new( name, code );
}
