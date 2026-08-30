namespace gestionAdminTECOCApi.Application.Features.Groups;

public record GroupResponse(
    Guid Id,
    string Name,
    string Code,
    bool Enabled
);
