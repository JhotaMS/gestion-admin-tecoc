namespace gestionAdminTECOCApi.Application.Exceptions;

public class NotFoundApplicationException : ApplicationException {
    public NotFoundApplicationException(
        string name,
        object key
    ) : base( $"Entity \"{name}\" ({key}) no fue encontrado" ) {

    }
}
