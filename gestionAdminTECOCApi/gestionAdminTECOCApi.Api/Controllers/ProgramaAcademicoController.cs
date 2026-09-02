using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.ProgramasAcademicos;
using gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.CreateProgramaAcademico;
using gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.DeleteProgramaAcademico;
using gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.GetAllProgramasAcademicos;
using gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.UpdateProgramaAcademico;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class ProgramaAcademicoController(
    IDispatch dispatch
) : ControllerBase {
    [HttpGet]
    [ProducesResponseType( typeof( GetAllProgramasAcademicosResponse ), (int)HttpStatusCode.OK )]
    public async Task<ActionResult<GetAllProgramasAcademicosResponse>> GetAllAsync(
        CancellationToken cancellationToken
    ) {
        Result<GetAllProgramasAcademicosResponse> result = await dispatch.Send(
            new GetAllProgramasAcademicosQuery(),
            cancellationToken
        );

        return Ok( result.Value );
    }

    [HttpPost]
    [ProducesResponseType( typeof( ProgramaAcademicoResponse ), (int)HttpStatusCode.Created )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.Conflict )]
    public async Task<ActionResult<ProgramaAcademicoResponse>> CreateAsync(
        [FromBody] CreateProgramaAcademicoCommand request,
        CancellationToken cancellationToken
    ) {
        Result<ProgramaAcademicoResponse> result = await dispatch.Send( request, cancellationToken );
        if (result.IsFailure)
            return ErrorResponse( result.Error );

        return StatusCode( (int)HttpStatusCode.Created, result.Value );
    }

    [HttpPut( "{programaAcademicoId:guid}" )]
    [ProducesResponseType( typeof( ProgramaAcademicoResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.NotFound )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.Conflict )]
    public async Task<ActionResult<ProgramaAcademicoResponse>> UpdateAsync(
        Guid programaAcademicoId,
        [FromBody] UpdateProgramaAcademicoCommand request,
        CancellationToken cancellationToken
    ) {
        if (programaAcademicoId != request.ProgramaAcademicoId) {
            return BadRequest( new CodeError(
                (int)HttpStatusCode.BadRequest,
                "El id de la ruta no coincide con el del cuerpo de la solicitud"
            ) );
        }

        Result<ProgramaAcademicoResponse> result = await dispatch.Send( request, cancellationToken );
        if (result.IsFailure)
            return ErrorResponse( result.Error );

        return Ok( result.Value );
    }

    [HttpDelete( "{programaAcademicoId:guid}" )]
    [ProducesResponseType( (int)HttpStatusCode.NoContent )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.NotFound )]
    public async Task<IActionResult> DeleteAsync(
        Guid programaAcademicoId,
        CancellationToken cancellationToken
    ) {
        Result result = await dispatch.Send(
            new DeleteProgramaAcademicoCommand( programaAcademicoId ),
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
        "ProgramaAcademico.NotFound" => (int)HttpStatusCode.NotFound,
        "ProgramaAcademico.DuplicateCode" => (int)HttpStatusCode.Conflict,
        _ => (int)HttpStatusCode.BadRequest
    };
}
