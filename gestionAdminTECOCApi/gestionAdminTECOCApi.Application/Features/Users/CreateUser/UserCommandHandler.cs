using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Users.CreateUser;

internal sealed class UserCommandHandler(
    UserService userService
) : ICommandHandler<UserCommand, UserCommandResponse> {

    public async Task<Result<UserCommandResponse>> Handle( UserCommand request
        , CancellationToken cancellationToken
    ) {
        if (!DocumentTypeCodes.TryParse( request.DocumentType, out DocumentType documentType )) {
            return Result.Failure<UserCommandResponse>(
                UserErrors.DocumentTypeNotAllowed( request.DocumentType )
            );
        }

        string documentNumber = request.DocumentNumber.Trim();

        bool documentAlreadyRegistered = await userService
            .ExistsByDocumentAsync(
                documentType
                , documentNumber
                , cancellationToken
            );

        if (documentAlreadyRegistered) {
            return Result.Failure<UserCommandResponse>(
                UserErrors.DocumentAlreadyRegistered(
                    DocumentTypeCodes.ToCode( documentType )
                    , documentNumber
                )
            );
        }

        User user = User.Create(
            request.FullName.Trim()
            , documentType
            , documentNumber
            , request.Position.Trim()
        );

        Guid id = await userService
            .CreateUserAsync( user, cancellationToken );

        return new UserCommandResponse(
            id
            , user.FullName
            , DocumentTypeCodes.ToCode( user.DocumentType )
            , user.DocumentNumber
            , user.Position
        );
    }
}
