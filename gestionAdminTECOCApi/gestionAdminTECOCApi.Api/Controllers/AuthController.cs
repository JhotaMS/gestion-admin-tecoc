using gestionAdminTECOCApi.Application.Features.Auth.Login;
using gestionAdminTECOCApi.Application.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class AuthController(
    IDispatch dispatch
) : ControllerBase {

    [HttpPost( "login" )]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginCommand request,
        CancellationToken cancellationToken
    ) {
        var result = await dispatch.Send( request, cancellationToken );
        if (result.IsFailure) {
            return Unauthorized();
        }
        return Ok( result.Value );
    }
}
