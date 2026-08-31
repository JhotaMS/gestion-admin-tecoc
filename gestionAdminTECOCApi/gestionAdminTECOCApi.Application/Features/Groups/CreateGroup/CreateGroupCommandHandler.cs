using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Groups;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.Groups.CreateGroup;

internal sealed class CreateGroupCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateGroupCommand, GroupResponse> {
    public async Task<Result<GroupResponse>> Handle(
        CreateGroupCommand request,
        CancellationToken cancellationToken
    ) {
        Error? validationError = GroupRequestValidation.Validate( request.Name, request.Code, request.CupoTotal );
        if (validationError is not null)
            return Result.Failure<GroupResponse>( validationError );

        string normalizedCode = Group.NormalizeCode( request.Code );
        var repository = unitOfWork.Repository<Group>();
        IReadOnlyList<Group> existing = await repository.GetAsync(
            group => group.Code == normalizedCode,
            cancellationToken
        );

        if (existing.Any())
            return Result.Failure<GroupResponse>( GroupErrors.DuplicateCode );

        Group group = Group.Create( request.Name, request.Code, request.CupoTotal );
        await repository.AddAsync( group, cancellationToken );
        await unitOfWork.SaveChangesAsync();

        return Result.Success( ToResponse( group, cupoDisponible: group.CupoTotal ) );
    }

    private static GroupResponse ToResponse( Group group, int cupoDisponible ) => new(
        group.Id,
        group.Name,
        group.Code,
        group.Enabled,
        group.CupoTotal,
        cupoDisponible
    );
}
