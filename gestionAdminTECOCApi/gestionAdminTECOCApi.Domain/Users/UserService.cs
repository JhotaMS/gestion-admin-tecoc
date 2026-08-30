using gestionAdminTECOCApi.Domain.DomainService;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Abstractions;


namespace gestionAdminTECOCApi.Domain.Users;

[DomainService]
public class UserService( IUnitOfWork unitOfWork ) {

    // ... CreateUserAsync y ExistsByDocumentAsync ya existentes ...

    public async Task<Result> DeleteUserAsync(
        Guid id,
        CancellationToken cancellationToken
    ) {
        User user = await unitOfWork.Repository<User>().GetByAsync(
            u => u.Id == id,
            disableTracking: false,
            cancellationToken
        );

        if (user is null) {
            return Result.Failure( UserErrors.NotFound( id ) );
        }

        if (!user.Enabled) {
            return Result.Failure( UserErrors.AlreadyDisabled( id ) );
        }

        user.Disable();

        await unitOfWork.Repository<User>().UpdateAsync( user );

        return Result.Success();
    }
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
