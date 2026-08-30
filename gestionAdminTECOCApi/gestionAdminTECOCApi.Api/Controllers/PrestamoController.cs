using System.Net;
using gestionAdminTECOCApi.Application.Features.Prestamos.GetPagedPrestamos;
using gestionAdminTECOCApi.Application.Features.Prestamos.GetPrestamoById;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace gestionAdminTECOCApi.Api.Controllers;

[Route( "api/v1/[controller]" )]
public class PrestamoController(
    IDispatch dispatch
) : ControllerBase {

    // GET /api/v1/Prestamo?pageNumber=1&pageSize=10
    // pageNumber y pageSize son opcionales (nullable): si no se envían, se usa página 1 con 10
    // registros. Si se envían con un valor inválido (ej. pageNumber=0), el query los valida y
    // responde 400 en vez de aplicar el valor por defecto silenciosamente.
    // Los [ProducesResponseType] son los que le dicen a Swagger la forma exacta de la
    // respuesta (incluidos los campos de nombre resueltos); sin ellos, Swagger documenta
    // el 200 sin ningún detalle aunque el endpoint sí devuelva el JSON completo.
    [HttpGet()]
    [ProducesResponseType( typeof( GetPagedPrestamosResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( Error ), (int)HttpStatusCode.BadRequest )]
    public async Task<IActionResult> GetPagedAsync(
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken
    ) {
        var query = new GetPagedPrestamosQuery( pageNumber ?? 1, pageSize ?? 10 );

        var result = await dispatch.Send( query, cancellationToken );
        if (result.IsFailure)
            return BadRequest( result.Error );
        return Ok( result.Value );
    }

    // GET /api/v1/Prestamo/{id}
    [HttpGet( "{id:guid}" )]
    [ProducesResponseType( typeof( GetPrestamoByIdResponse ), (int)HttpStatusCode.OK )]
    [ProducesResponseType( typeof( Error ), (int)HttpStatusCode.NotFound )]
    public async Task<IActionResult> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    ) {
        var query = new GetPrestamoByIdQuery( id );

        var result = await dispatch.Send( query, cancellationToken );
        if (result.IsFailure)
            return NotFound( result.Error );
        return Ok( result.Value );
    }
}
