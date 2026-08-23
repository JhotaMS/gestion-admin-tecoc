namespace gestionAdminTECOCApi.Application.Features.Users.CreateUser;

public record UserCommandResponse(
    Guid Id
    , string FullName
    , string DocumentType
    , string DocumentNumber
    , string Position
);
