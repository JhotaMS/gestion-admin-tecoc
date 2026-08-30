using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.Users.DeleteUser;

public record DeleteUserCommand( Guid Id ) : ICommand;