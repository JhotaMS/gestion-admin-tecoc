using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Groups;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.ProgramasAcademicos;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Users.UpdateUser;

internal sealed class UpdateUserCommandHandler(
    UserService userService
    , IUnitOfWork unitOfWork
) : ICommandHandler<UpdateUserCommand, UpdateUserCommandResponse> {

    public async Task<Result<UpdateUserCommandResponse>> Handle( UpdateUserCommand request
        , CancellationToken cancellationToken
    ) {
        if (!DocumentTypeCodes.TryParse( request.DocumentType, out DocumentType documentType )) {
            return Result.Failure<UpdateUserCommandResponse>(
                UserErrors.DocumentTypeNotAllowed( request.DocumentType )
            );
        }

        User user = await userService.GetByIdAsync( request.Id, cancellationToken );
        if (user is null) {
            return Result.Failure<UpdateUserCommandResponse>(
                UserErrors.NotFound( request.Id )
            );
        }

        string documentNumber = request.DocumentNumber.Trim();

        bool documentUsedByAnotherUser = await userService
            .ExistsByDocumentExcludingUserAsync(
                request.Id
                , documentType
                , documentNumber
                , cancellationToken
            );

        if (documentUsedByAnotherUser) {
            return Result.Failure<UpdateUserCommandResponse>(
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
                return Result.Failure<UpdateUserCommandResponse>( GroupErrors.NotFound );
        }

        if (request.ProgramaAcademicoId is not null) {
            bool programaExists = await unitOfWork.Repository<ProgramaAcademico>()
                .Exitst( programa => programa.Id == request.ProgramaAcademicoId, cancellationToken );

            if (!programaExists)
                return Result.Failure<UpdateUserCommandResponse>( ProgramaAcademicoErrors.NotFound );
        }

        user.Update(
            request.FullName.Trim()
            , documentType
            , documentNumber
            , request.UserName.Trim()
            , request.Email.Trim()
        );

        user.AssignGroup( request.GroupId );
        user.AssignProgramaAcademico( request.ProgramaAcademicoId );

        await userService.UpdateUserAsync( user, cancellationToken );

        return new UpdateUserCommandResponse(
            user.Id
            , user.FullName
            , DocumentTypeCodes.ToCode( user.DocumentType )
            , user.DocumentNumber
            , user.UserName
            , user.Email
        );
    }
}
