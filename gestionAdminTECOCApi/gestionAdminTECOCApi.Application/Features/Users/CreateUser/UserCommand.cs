using gestionAdminTECOCApi.Application.Messaging;
using System.ComponentModel.DataAnnotations;

namespace gestionAdminTECOCApi.Application.Features.Users.CreateUser;

public record UserCommand(
    [Required] string FullName
    , [Required] string DocumentType
    , [Required] string DocumentNumber
    , [Required] string UserName
    , [Required] string Email
    , [Required] string Password
    ) : ICommand<UserCommandResponse>;
