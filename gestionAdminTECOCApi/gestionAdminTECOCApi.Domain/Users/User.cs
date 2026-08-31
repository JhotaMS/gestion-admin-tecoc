using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Groups;
using gestionAdminTECOCApi.Domain.ProgramasAcademicos;

namespace gestionAdminTECOCApi.Domain.Users;

public class User : Entity<Guid> {
    private User(
        string fullName,
        DocumentType documentType,
        string documentNumber,
        string userName,
        string email,
        string passwordHash,
        Guid? groupId,
        Guid? programaAcademicoId
    ) : base( true ) {
        Id = Guid.NewGuid();
        FullName = fullName;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
        GroupId = groupId;
        ProgramaAcademicoId = programaAcademicoId;
    }

    public string FullName { get; private set; } = default!;
    public DocumentType DocumentType { get; private set; }
    public string DocumentNumber { get; private set; } = default!;
    public string UserName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public Guid? GroupId { get; private set; }
    public Group? Group { get; private set; }
    public Guid? ProgramaAcademicoId { get; private set; }
    public ProgramaAcademico? ProgramaAcademico { get; private set; }

    public static User Create(
        string fullName,
        DocumentType documentType,
        string documentNumber,
        string userName,
        string email,
        string passwordHash,
        Guid? groupId = null,
        Guid? programaAcademicoId = null
    ) => new(
        fullName,
        documentType,
        documentNumber,
        userName,
        email,
        passwordHash,
        groupId,
        programaAcademicoId
    );

    public void Update(
        string fullName,
        DocumentType documentType,
        string documentNumber,
        string userName,
        string email
    ) {
        FullName = fullName;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        UserName = userName;
        Email = email;
    }

    public void AssignGroup( Guid? groupId ) {
        GroupId = groupId;
    }

    public void AssignProgramaAcademico( Guid? programaAcademicoId ) {
        ProgramaAcademicoId = programaAcademicoId;
    }
}
