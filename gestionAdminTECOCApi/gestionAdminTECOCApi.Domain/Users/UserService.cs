using gestionAdminTECOCApi.Domain.DomainService;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Domain.Users;

[DomainService]
public class UserService(
    IUnitOfWork unitOfWork
) {

    public async Task<Guid> CreateUserAsync(
        User user,
        CancellationToken cancellationToken
    ) {
        ArgumentNullException.ThrowIfNull( user );

        await unitOfWork.Repository<User>()
            .AddAsync( user, cancellationToken );

        return user.Id;
    }

    public async Task<bool> ExistsByDocumentAsync(
        DocumentType documentType,
        string documentNumber,
        CancellationToken cancellationToken
    ) => await unitOfWork.Repository<User>()
        .Exitst(
            user => user.DocumentType == documentType
                && user.DocumentNumber == documentNumber,
            cancellationToken
        );
}
