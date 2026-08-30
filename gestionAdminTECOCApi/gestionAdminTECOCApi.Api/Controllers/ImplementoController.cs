using gestionAdminTECOCApi.Application.Features.Implementos.GetAllImplementos;
using gestionAdminTECOCApi.Application.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class ImplementoController(
    IDispatch dispatch
) : ControllerBase {

    [HttpGet()]
    public async Task<IActionResult> GetAllAsync(
        CancellationToken cancellationToken
    ) {
        var result = await dispatch.Send( new GetAllImplementosQuery(), cancellationToken );
        if (result.IsFailure) return BadRequest( result.Error );
        return Ok( result.Value );
    }
}
