using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.Loans.GetImplementos;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class ImplementosController(
    IDispatch dispatch
) : ControllerBase {

    [HttpGet()]
    [ProducesResponseType( typeof( GetAllImplementosResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    public async Task<ActionResult<GetAllImplementosResponse>> GetAllAsync(
        CancellationToken cancellationToken
    ) {
        Result<GetAllImplementosResponse> result = await dispatch.Send(
            new GetAllImplementosQuery(),
            cancellationToken
        );

        if (result.IsFailure) {
            return BadRequest( new CodeError( (int)HttpStatusCode.BadRequest, result.Error.Name ) );
        }

        return Ok( result.Value );
    }
}

