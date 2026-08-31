using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.Groups.CreateGroup;

public record CreateGroupCommand(
    string Name,
    string Code,
    int CupoTotal
) : ICommand<GroupResponse>;
