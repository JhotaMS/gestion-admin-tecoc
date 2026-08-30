using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.Loans.GetTiposRevision;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class TiposRevisionController(
    IDispatch dispatch
) : ControllerBase {

    [HttpGet()]
    [ProducesResponseType( typeof( GetAllTiposRevisionResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    public async Task<ActionResult<GetAllTiposRevisionResponse>> GetAllAsync(
        CancellationToken cancellationToken
    ) {
        Result<GetAllTiposRevisionResponse> result = await dispatch.Send(
            new GetAllTiposRevisionQuery(),
            cancellationToken
        );

        if (result.IsFailure) {
            return BadRequest( new CodeError( (int)HttpStatusCode.BadRequest, result.Error.Name ) );
        }

        return Ok( result.Value );
    }
}

