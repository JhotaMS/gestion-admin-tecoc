using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.DomainService;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Domain.Users;

[DomainService]
public class UserService(
    IUnitOfWork unitOfWork
) {

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
            return Result.Failure( UserErrors.NotFound( id ) );
        }

        user.Disable();

        await unitOfWork.Repository<User>().UpdateAsync( user );

        return Result.Success();
    }

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

    public async Task<bool> ExistsByDocumentExcludingUserAsync(
        Guid userId,
        DocumentType documentType,
        string documentNumber,
        CancellationToken cancellationToken
    ) => await unitOfWork.Repository<User>()
        .Exitst(
            user => user.Id != userId
                && user.DocumentType == documentType
                && user.DocumentNumber == documentNumber,
            cancellationToken
        );

    public async Task<User> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken
    ) => await unitOfWork.Repository<User>()
        .GetByAsync( user => user.Id == userId, disableTracking: false, cancellationToken );

    public Task UpdateUserAsync(
        User user,
        CancellationToken cancellationToken
    ) {
        ArgumentNullException.ThrowIfNull( user );

        // El UnitOfWorkBehevior del pipeline hace el SaveChangesAsync tras el Handle.
        unitOfWork.Repository<User>().Update( user );

        return Task.CompletedTask;
    }
}
