using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.Implementos.Queries.ImplementoList;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "v1/[controller]" )]
public class ImplementosController(
    ILogger<ImplementosController> logger,
    IDispatch dispatch
) : ControllerBase {

    [HttpGet( "disponibles" )]
    [ProducesResponseType( typeof( ImplementoListResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    public async Task<ActionResult<ImplementoListResponse>> GetDisponiblesAsync(
        CancellationToken cancellationToken
    ) {
        logger.LogInformation(
            "En la siguiente fecha {date} a las {time}, se llamo el endpoint {endpoint} de la clase {class}",
                DateTime.Now.ZoneByIdPacificStandardTime().ToString( "dd/MM/yyyy", provider: new CultureInfo( "es-CO" ) ),
                DateTime.Now.ZoneByIdPacificStandardTime().ToString( "hh:mm tt" ),
                nameof( GetDisponiblesAsync ),
                nameof( ImplementosController )
        );

        Result<ImplementoListResponse> result = await dispatch.Send(
            new ImplementoListQuery(),
            cancellationToken
        );

        if (result.IsFailure) {
            return StatusCode(
                (int)HttpStatusCode.BadRequest,
                new CodeError( (int)HttpStatusCode.BadRequest, result.Error.Name )
            );
        }

        return StatusCode( (int)HttpStatusCode.OK, result.Value );
    }
}
