using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.CalendarioAcademico;
using gestionAdminTECOCApi.Application.Features.CalendarioAcademico.CreateEventoAcademico;
using gestionAdminTECOCApi.Application.Features.CalendarioAcademico.DeleteEventoAcademico;
using gestionAdminTECOCApi.Application.Features.CalendarioAcademico.GetAllEventosAcademicos;
using gestionAdminTECOCApi.Application.Features.CalendarioAcademico.UpdateEventoAcademico;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class CalendarioAcademicoController(
    IDispatch dispatch
) : ControllerBase {
    [HttpGet]
    [ProducesResponseType( typeof( GetAllEventosAcademicosResponse ), (int)HttpStatusCode.OK )]
    public async Task<ActionResult<GetAllEventosAcademicosResponse>> GetAllAsync(
        CancellationToken cancellationToken
    ) {
        Result<GetAllEventosAcademicosResponse> result = await dispatch.Send(
            new GetAllEventosAcademicosQuery(),
            cancellationToken
        );

        return Ok( result.Value );
    }

    [HttpPost]
    [ProducesResponseType( typeof( EventoAcademicoResponse ), (int)HttpStatusCode.Created )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    public async Task<ActionResult<EventoAcademicoResponse>> CreateAsync(
        [FromBody] CreateEventoAcademicoCommand request,
        CancellationToken cancellationToken
    ) {
        Result<EventoAcademicoResponse> result = await dispatch.Send( request, cancellationToken );
        if (result.IsFailure)
            return ErrorResponse( result.Error );

        return StatusCode( (int)HttpStatusCode.Created, result.Value );
    }

    [HttpPut( "{eventoAcademicoId:guid}" )]
    [ProducesResponseType( typeof( EventoAcademicoResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.NotFound )]
    public async Task<ActionResult<EventoAcademicoResponse>> UpdateAsync(
        Guid eventoAcademicoId,
        [FromBody] UpdateEventoAcademicoCommand request,
        CancellationToken cancellationToken
    ) {
        if (eventoAcademicoId != request.EventoAcademicoId) {
            return BadRequest( new CodeError(
                (int)HttpStatusCode.BadRequest,
                "El id de la ruta no coincide con el del cuerpo de la solicitud"
            ) );
        }

        Result<EventoAcademicoResponse> result = await dispatch.Send( request, cancellationToken );
        if (result.IsFailure)
            return ErrorResponse( result.Error );

        return Ok( result.Value );
    }

    [HttpDelete( "{eventoAcademicoId:guid}" )]
    [ProducesResponseType( (int)HttpStatusCode.NoContent )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.NotFound )]
    public async Task<IActionResult> DeleteAsync(
        Guid eventoAcademicoId,
        CancellationToken cancellationToken
    ) {
        Result result = await dispatch.Send(
            new DeleteEventoAcademicoCommand( eventoAcademicoId ),
            cancellationToken
        );

        if (result.IsFailure)
            return ErrorResponse( result.Error );

        return NoContent();
    }

    private ObjectResult ErrorResponse( Error error ) {
        int statusCode = StatusCodeByError( error );
        return StatusCode( statusCode, new CodeError( statusCode, error.Name ) );
    }

    private static int StatusCodeByError( Error error ) => error.Code switch {
        "EventoAcademico.NotFound" => (int)HttpStatusCode.NotFound,
        _ => (int)HttpStatusCode.BadRequest
    };
}
