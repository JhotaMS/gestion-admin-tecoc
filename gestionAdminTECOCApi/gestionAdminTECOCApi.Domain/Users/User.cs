using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Users;

public class User : Entity<Guid> {
    private User(
        string firstName,
        string? secondName,
        string surName,
        string? secondSurName
    ) {
        FirstName = firstName;
        SecondName = secondName;
        SurName = surName;
        SecondSurName = secondSurName;
    }

    public string FirstName { get; private set; } = default!;
    public string? SecondName { get; private set; }
    public string SurName { get; private set; } = default!;
    public string? SecondSurName { get; private set; }

    public static User Create(
        string firstName,
        string? secondName,
        string surName,
        string? secondSurname
    ) => new(
        firstName,
        secondName,
        surName,
        secondSurname
    );
}
