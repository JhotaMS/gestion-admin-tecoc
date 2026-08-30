using gestionAdminTECOCApi.Application.Features.Implementos.GetImplementosDisponibles;
using gestionAdminTECOCApi.Application.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/implementos" )]
public class ImplementosController(
    IDispatch dispatch
) : ControllerBase {

    [HttpGet( "disponibles" )]
    public async Task<IActionResult> GetDisponiblesAsync(
        CancellationToken cancellationToken
    ) {
        var result = await dispatch.Send( new GetImplementosDisponiblesQuery(), cancellationToken );
        if (result.IsFailure) return BadRequest( result.Error );
        return Ok( result.Value );
    }
}
