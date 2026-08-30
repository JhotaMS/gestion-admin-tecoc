using gestionAdminTECOCApi.Application.Features.Users.GetUsers;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Users.GetUserById;

public record GetUserByIdQuery( Guid UserId ) : IQuery<GetUserByIdResponse>;

public record GetUserByIdResponse( UserDto User );

internal sealed class GetUserByIdQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetUserByIdQuery, GetUserByIdResponse> {
    public async Task<Result<GetUserByIdResponse>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken
    ) {
        var users = await unitOfWork
            .Repository<User>()
            .GetAsync(
                user => user.Id == request.UserId,
                orderBy: null,
                includeString: "Group",
                disableTracking: true,
                cancellationToken: cancellationToken
            );

        User? user = users.SingleOrDefault();
        if (user is null) {
            return Result.Failure<GetUserByIdResponse>(
                new Error( "User.NotFound", "No se encontró el usuario solicitado" )
            );
        }

        var dto = new UserDto(
            user.Id,
            user.FullName,
            user.UserName,
            user.Email,
            user.Enabled,
            user.Group is null
                ? null
                : new GroupDto( user.Group.Id, user.Group.Name, user.Group.Code )
        );

        return Result.Success( new GetUserByIdResponse( dto ) );
    }
}
