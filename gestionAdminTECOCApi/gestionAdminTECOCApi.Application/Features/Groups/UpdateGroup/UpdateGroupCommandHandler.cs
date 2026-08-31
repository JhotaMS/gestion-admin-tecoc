using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Groups;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Groups.UpdateGroup;

internal sealed class UpdateGroupCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateGroupCommand, GroupResponse> {
    public async Task<Result<GroupResponse>> Handle(
        UpdateGroupCommand request,
        CancellationToken cancellationToken
    ) {
        Error? validationError = GroupRequestValidation.Validate( request.Name, request.Code, request.CupoTotal );
        if (validationError is not null)
            return Result.Failure<GroupResponse>( validationError );

        var repository = unitOfWork.Repository<Group>();
        IReadOnlyList<Group> matches = await repository.GetAsync(
            group => group.Id == request.GroupId,
            orderBy: null,
            includeString: null,
            disableTracking: false,
            cancellationToken: cancellationToken
        );

        Group? group = matches.SingleOrDefault();
        if (group is null)
            return Result.Failure<GroupResponse>( GroupErrors.NotFound );

        string normalizedCode = Group.NormalizeCode( request.Code );
        IReadOnlyList<Group> duplicateCodes = await repository.GetAsync(
            candidate => candidate.Id != request.GroupId && candidate.Code == normalizedCode,
            cancellationToken
        );

        if (duplicateCodes.Any())
            return Result.Failure<GroupResponse>( GroupErrors.DuplicateCode );

        group.Update( request.Name, request.Code, request.CupoTotal );
        await unitOfWork.SaveChangesAsync();

        int matriculados = ( await unitOfWork.Repository<User>().GetAsync(
            user => user.GroupId == group.Id && user.Enabled,
            cancellationToken
        ) ).Count;

        return Result.Success( new GroupResponse(
            group.Id,
            group.Name,
            group.Code,
            group.Enabled,
            group.CupoTotal,
            Math.Max( 0, group.CupoTotal - matriculados )
        ) );
    }
}
