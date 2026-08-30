using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.Groups;
using gestionAdminTECOCApi.Application.Features.Groups.CreateGroup;
using gestionAdminTECOCApi.Application.Features.Groups.DeleteGroup;
using gestionAdminTECOCApi.Application.Features.Groups.GetAllGroups;
using gestionAdminTECOCApi.Application.Features.Groups.UpdateGroup;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class GroupController(
    IDispatch dispatch
) : ControllerBase {
    [HttpGet]
    [ProducesResponseType( typeof( GetAllGroupsResponse ), (int)HttpStatusCode.OK )]
    public async Task<ActionResult<GetAllGroupsResponse>> GetAllAsync(
        CancellationToken cancellationToken
    ) {
        Result<GetAllGroupsResponse> result = await dispatch.Send(
            new GetAllGroupsQuery(),
            cancellationToken
        );

        return Ok( result.Value );
    }

    [HttpPost]
    [ProducesResponseType( typeof( GroupResponse ), (int)HttpStatusCode.Created )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.Conflict )]
    public async Task<ActionResult<GroupResponse>> CreateAsync(
        [FromBody] CreateGroupCommand request,
        CancellationToken cancellationToken
    ) {
        Result<GroupResponse> result = await dispatch.Send( request, cancellationToken );
        if (result.IsFailure)
            return ErrorResponse( result.Error );

        return StatusCode( (int)HttpStatusCode.Created, result.Value );
    }

    [HttpPut( "{groupId:guid}" )]
    [ProducesResponseType( typeof( GroupResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.BadRequest )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.NotFound )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.Conflict )]
    public async Task<ActionResult<GroupResponse>> UpdateAsync(
        Guid groupId,
        [FromBody] UpdateGroupCommand request,
        CancellationToken cancellationToken
    ) {
        if (groupId != request.GroupId) {
            return BadRequest( new CodeError(
                (int)HttpStatusCode.BadRequest,
                "El id de la ruta no coincide con el del cuerpo de la solicitud"
            ) );
        }

        Result<GroupResponse> result = await dispatch.Send( request, cancellationToken );
        if (result.IsFailure)
            return ErrorResponse( result.Error );

        return Ok( result.Value );
    }

    [HttpDelete( "{groupId:guid}" )]
    [ProducesResponseType( (int)HttpStatusCode.NoContent )]
    [ProducesResponseType( typeof( CodeError ), (int)HttpStatusCode.NotFound )]
    public async Task<IActionResult> DeleteAsync(
        Guid groupId,
        CancellationToken cancellationToken
    ) {
        Result result = await dispatch.Send(
            new DeleteGroupCommand( groupId ),
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
        "Group.NotFound" => (int)HttpStatusCode.NotFound,
        "Group.DuplicateCode" => (int)HttpStatusCode.Conflict,
        _ => (int)HttpStatusCode.BadRequest
    };
}
