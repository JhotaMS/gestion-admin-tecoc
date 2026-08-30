using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Groups;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.Groups.DeleteGroup;

public record DeleteGroupCommand(
    Guid GroupId
) : ICommand;

internal sealed class DeleteGroupCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<DeleteGroupCommand> {
    public async Task<Result> Handle(
        DeleteGroupCommand request,
        CancellationToken cancellationToken
    ) {
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
            return Result.Failure( GroupErrors.NotFound );

        repository.Delete( group );
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
