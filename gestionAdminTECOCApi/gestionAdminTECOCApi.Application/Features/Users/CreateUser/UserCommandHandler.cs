using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Groups;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.ProgramasAcademicos;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Users.CreateUser;

internal sealed class UserCommandHandler(
    UserService userService
    , IUnitOfWork unitOfWork
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

        if (request.GroupId is not null) {
            bool groupExists = await unitOfWork.Repository<Group>()
                .Exitst( group => group.Id == request.GroupId, cancellationToken );

            if (!groupExists)
                return Result.Failure<UserCommandResponse>( GroupErrors.NotFound );
        }

        if (request.ProgramaAcademicoId is not null) {
            bool programaExists = await unitOfWork.Repository<ProgramaAcademico>()
                .Exitst( programa => programa.Id == request.ProgramaAcademicoId, cancellationToken );

            if (!programaExists)
                return Result.Failure<UserCommandResponse>( ProgramaAcademicoErrors.NotFound );
        }

        User user = User.Create(
            request.FullName.Trim()
            , documentType
            , documentNumber
            , request.UserName.Trim()
            , request.Email.Trim()
            , PasswordHasher.Hash( request.Password )
            , request.GroupId
            , request.ProgramaAcademicoId
        );

        Guid id = await userService
            .CreateUserAsync( user, cancellationToken );

        return new UserCommandResponse(
            id
            , user.FullName
            , DocumentTypeCodes.ToCode( user.DocumentType )
            , user.DocumentNumber
            , user.UserName
            , user.Email
        );
    }
}
