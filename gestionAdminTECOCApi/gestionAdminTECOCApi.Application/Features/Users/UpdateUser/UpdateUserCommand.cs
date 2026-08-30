using gestionAdminTECOCApi.Application.Messaging;
using System.ComponentModel.DataAnnotations;

namespace gestionAdminTECOCApi.Application.Features.Users.UpdateUser;

public record UpdateUserCommand(
    [Required] Guid Id
    , [Required] string FullName
    , [Required] string DocumentType
    , [Required] string DocumentNumber
    , [Required] string UserName
    , [Required] string Email
    ) : ICommand<UpdateUserCommandResponse>;
