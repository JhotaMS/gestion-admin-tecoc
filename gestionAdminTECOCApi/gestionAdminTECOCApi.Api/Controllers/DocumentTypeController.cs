using gestionAdminTECOCApi.Application.Features.DocumentTypes.CreateDocumentType;
using gestionAdminTECOCApi.Application.Features.DocumentTypes.DeleteDocumentType;
using gestionAdminTECOCApi.Application.Features.DocumentTypes.GetAllDocumentTypes;
using gestionAdminTECOCApi.Application.Features.DocumentTypes.UpdateDocumentType;
using gestionAdminTECOCApi.Application.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class DocumentTypeController(
    IDispatch dispatch
) : ControllerBase {

    [HttpPost()]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateDocumentTypeCommand request,
        CancellationToken cancellationToken
    ) {
        var result = await dispatch.Send( request, cancellationToken );
        if (result.IsFailure)
            return BadRequest( result.Error );
        return Ok( result.Value );
    }

    [HttpPut( "{documentTypeId:guid}" )]
    public async Task<IActionResult> UpdateAsync(
        Guid documentTypeId,
        [FromBody] UpdateDocumentTypeCommand request,
        CancellationToken cancellationToken
    ) {
        if (documentTypeId != request.DocumentTypeId) {
            return BadRequest( new { message = "El DocumentTypeId de la ruta no coincide con el cuerpo" } );
        }
        var result = await dispatch.Send( request, cancellationToken );
        if (result.IsFailure)
            return BadRequest( result.Error );
        return Ok();
    }

    [HttpDelete( "{documentTypeId:guid}" )]
    public async Task<IActionResult> DeleteAsync(
        Guid documentTypeId,
        CancellationToken cancellationToken
    ) {
        var result = await dispatch.Send( new DeleteDocumentTypeCommand( documentTypeId ), cancellationToken );
        if (result.IsFailure)
            return BadRequest( result.Error );
        return Ok();
    }

    [HttpGet()]
    public async Task<IActionResult> GetAllAsync(
        CancellationToken cancellationToken
    ) {
        var result = await dispatch.Send( new GetAllDocumentTypesQuery(), cancellationToken );
        if (result.IsFailure)
            return BadRequest( result.Error );
        return Ok( result.Value );
    }
}
