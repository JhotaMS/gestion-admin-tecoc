using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.Loans.CreateImplementoPrestado;
using gestionAdminTECOCApi.Application.Features.Loans.GetAllImplementosPrestados;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class ImplementosPrestadosController(
    IDispatch dispatch
) : ControllerBase {

    [HttpPost()]
    [ProducesResponseType( typeof( CreateImplementoPrestadoResponse ), (int)HttpStatusCode.Created )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.NotFound )]
    public async Task<ActionResult<CreateImplementoPrestadoResponse>> CreateAsync(
        [FromBody] CreateImplementoPrestadoCommand request,
        CancellationToken cancellationToken
    ) {
        Result<CreateImplementoPrestadoResponse> result = await dispatch.Send(
            request,
            cancellationToken
        );

        if (result.IsFailure) {
            int statusCode = result.Error.Code switch {
                "ImplementoPrestado.UserNotFound" => (int)HttpStatusCode.NotFound,
                "ImplementoPrestado.ImplementoNotFound" => (int)HttpStatusCode.NotFound,
                "ImplementoPrestado.NotFound" => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.BadRequest
            };

            return StatusCode(
                statusCode,
                new CodeError( statusCode, result.Error.Name )
            );
        }

        return StatusCode( (int)HttpStatusCode.Created, result.Value );
    }

    [HttpGet()]
    [ProducesResponseType( typeof( GetAllImplementosPrestadosResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    public async Task<ActionResult<GetAllImplementosPrestadosResponse>> GetAllAsync(
        CancellationToken cancellationToken
    ) {
        Result<GetAllImplementosPrestadosResponse> result = await dispatch.Send(
            new GetAllImplementosPrestadosQuery(),
            cancellationToken
        );

        if (result.IsFailure) {
            return BadRequest( new CodeError( (int)HttpStatusCode.BadRequest, result.Error.Name ) );
        }

        return Ok( result.Value );
    }
}

