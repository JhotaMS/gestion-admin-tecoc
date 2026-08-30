namespace gestionAdminTECOCApi.Application.Features.Users.UpdateUser;

public record UpdateUserCommandResponse(
    Guid Id
    , string FullName
    , string DocumentType
    , string DocumentNumber
    , string UserName
    , string Email
);
