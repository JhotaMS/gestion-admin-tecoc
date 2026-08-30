using System.Net;

namespace gestionAdminTECOCApi.Application.Exceptions;

public class NotFoundException : ApplicationException {
    public int StatusCode { get; } = (int)HttpStatusCode.NotFound;

    public NotFoundException( string message )
        : base( message ) {
    }

    public NotFoundException( string message, Exception innerException )
        : base( message, innerException ) {
    }
}