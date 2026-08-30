using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.ScheduledClasses.CreateScheduledClass;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class ScheduledClassController(
    ILogger<ScheduledClassController> logger,
    IDispatch dispatch
) : ControllerBase {

    [HttpPost()]
    [ProducesResponseType( typeof( ScheduledClassCommandResponse ), (int)HttpStatusCode.Created )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.Conflict )]
    public async Task<ActionResult<ScheduledClassCommandResponse>> CreateScheduledClassAsync(
        [FromBody] ScheduledClassCommand request,
        CancellationToken cancellationToken
    ) {
        logger.LogInformation(
            "En la siguiente fecha {date} a las {time}, se llamo el endpoint {endpoint} de la clase {class}",
                DateTime.Now.ZoneByIdPacificStandardTime().ToString( "dd/MM/yyyy", provider: new CultureInfo( "es-CO" ) ),
                DateTime.Now.ZoneByIdPacificStandardTime().ToString( "hh:mm tt" ),
                nameof( CreateScheduledClassAsync ),
                nameof( ScheduledClassController )
        );

        Result<ScheduledClassCommandResponse> result = await dispatch.Send(
            request,
            cancellationToken
        );

        if (result.IsFailure) {
            return StatusCode(
                StatusCodeByError( result.Error ),
                new CodeError( StatusCodeByError( result.Error ), result.Error.Name )
            );
        }

        return StatusCode( (int)HttpStatusCode.Created, result.Value );
    }

    private static int StatusCodeByError( Error error ) => error.Code switch {
        "ScheduledClass.ScheduleAlreadyTaken" => (int)HttpStatusCode.Conflict,
        _ => (int)HttpStatusCode.BadRequest
    };
}
