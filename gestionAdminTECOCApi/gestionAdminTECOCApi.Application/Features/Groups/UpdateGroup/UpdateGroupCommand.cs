using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.Groups.UpdateGroup;

public record UpdateGroupCommand(
    Guid GroupId,
    string Name,
    string Code
) : ICommand<GroupResponse>;
