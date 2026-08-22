using gestionAdminTECOCApi.Application.Messaging;
using System.ComponentModel.DataAnnotations;

namespace gestionAdminTECOCApi.Application.Features.Users.CreateUser;

public record UserCommand(
    [Required] string FirstName
    , string? SecondName
    , [Required] string SurName
    , string? SecondSurName
    ) : ICommand<UserCommandResponse>;
