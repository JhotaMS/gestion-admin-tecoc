using gestionAdminTECOCApi.Application.Features.Auth.Login;
using gestionAdminTECOCApi.Application.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "v1/[controller]" )]
public class AuthController(
    IDispatch dispatch,
    ILogger<AuthController> logger
) : ControllerBase {

    [HttpPost( "login" )]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginCommand request,
        CancellationToken cancellationToken
    ) {
        var result = await dispatch.Send( request, cancellationToken );
        if (result.IsFailure) {
            if (result.Error.Code == "Auth.AccountLocked") return StatusCode( 423, result.Error );
            if (result.Error.Code == "Auth.InvalidCredentials") return Unauthorized( result.Error );
            return BadRequest( result.Error );
        }
        return Ok( result.Value );
    }
}
