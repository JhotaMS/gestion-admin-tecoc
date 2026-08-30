using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.Users.CreateUser;
using gestionAdminTECOCApi.Application.Features.Users.GetAllUsers;
using gestionAdminTECOCApi.Application.Features.Users.UpdateUser;
using gestionAdminTECOCApi.Application.Features.Users.GetPagedUsers;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class UserController(
    ILogger<UserController> logger,
    IDispatch dispatch
) : ControllerBase {

    [HttpPost()]
    [ProducesResponseType( typeof( UserCommandResponse ), (int)HttpStatusCode.Created )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.Conflict )]
    public async Task<ActionResult<UserCommandResponse>> CreateUserAsync(
        [FromBody] UserCommand request,
        CancellationToken cancellationToken
    ) {
        logger.LogInformation(
            "En la siguiente fecha {date} a las {time}, se llamo el endpoint {endpoint} de la clase {class}",
                DateTime.Now.ZoneByIdPacificStandardTime().ToString( "dd/MM/yyyy", provider: new CultureInfo( "es-CO" ) ),
                DateTime.Now.ZoneByIdPacificStandardTime().ToString( "hh:mm tt" ),
                nameof( CreateUserAsync ),
                nameof( UserController )
        );

        Result<UserCommandResponse> result = await dispatch.Send(
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

    [HttpPut( "{userId:guid}" )]
    [ProducesResponseType( typeof( UpdateUserCommandResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.NotFound )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.Conflict )]
    public async Task<ActionResult<UpdateUserCommandResponse>> UpdateUserAsync(
        Guid userId,
        [FromBody] UpdateUserCommand request,
        CancellationToken cancellationToken
    ) {
        if (userId != request.Id) {
            return StatusCode(
                (int)HttpStatusCode.BadRequest,
                new CodeError( (int)HttpStatusCode.BadRequest, "El id de la ruta no coincide con el del cuerpo de la solicitud" )
            );
        }

        logger.LogInformation(
            "En la siguiente fecha {date} a las {time}, se llamo el endpoint {endpoint} de la clase {class}",
                DateTime.Now.ZoneByIdPacificStandardTime().ToString( "dd/MM/yyyy", provider: new CultureInfo( "es-CO" ) ),
                DateTime.Now.ZoneByIdPacificStandardTime().ToString( "hh:mm tt" ),
                nameof( UpdateUserAsync ),
                nameof( UserController )
        );

        Result<UpdateUserCommandResponse> result = await dispatch.Send(
            request,
            cancellationToken
        );

        if (result.IsFailure) {
            return StatusCode(
                StatusCodeByError( result.Error ),
                new CodeError( StatusCodeByError( result.Error ), result.Error.Name )
            );
        }

        return StatusCode( (int)HttpStatusCode.OK, result.Value );
    }

    [HttpGet()]
    [ProducesResponseType( typeof( GetAllUsersResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    public async Task<ActionResult<GetAllUsersResponse>> GetAllAsync(
        CancellationToken cancellationToken
    ) {
        Result<GetAllUsersResponse> result = await dispatch.Send(
            new GetAllUsersQuery(),
            cancellationToken
        );

        if (result.IsFailure) {
            return StatusCode(
                StatusCodeByError( result.Error ),
                new CodeError( StatusCodeByError( result.Error ), result.Error.Name )
            );
        }

        return StatusCode( (int)HttpStatusCode.OK, result.Value );
    }

    // GET /api/v1/User/paged?pageNumber=1&pageSize=8
    // pageNumber y pageSize son opcionales (nullable): si no se envían, se usa página 1 con 10
    // registros. Si se envían con un valor inválido (ej. pageNumber=0), el query los valida y
    // responde 400 en vez de aplicar el valor por defecto silenciosamente.
    [HttpGet( "paged" )]
    [ProducesResponseType( typeof( GetPagedUsersResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    public async Task<ActionResult<GetPagedUsersResponse>> GetPagedAsync(
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken
    ) {
        var query = new GetPagedUsersQuery( pageNumber ?? 1, pageSize ?? 10 );

        Result<GetPagedUsersResponse> result = await dispatch.Send( query, cancellationToken );

        if (result.IsFailure) {
            return StatusCode(
                StatusCodeByError( result.Error ),
                new CodeError( StatusCodeByError( result.Error ), result.Error.Name )
            );
        }

        return StatusCode( (int)HttpStatusCode.OK, result.Value );
    }

    private static int StatusCodeByError( Error error ) => error.Code switch {
        "User.DocumentAlreadyRegistered" => (int)HttpStatusCode.Conflict,
        "User.NotFound" => (int)HttpStatusCode.NotFound,
        _ => (int)HttpStatusCode.BadRequest
    };
}
