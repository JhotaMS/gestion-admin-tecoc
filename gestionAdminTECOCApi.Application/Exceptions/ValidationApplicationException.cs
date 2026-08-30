using FluentValidation.Results;

namespace gestionAdminTECOCApi.Application.Exceptions;

public class ValidationApplicationException : ApplicationException {
    public IDictionary<string, string[]> Errors { get; set; }
    public ValidationApplicationException() : base( "Se ha presentado uno o más errores de validación" ) {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationApplicationException( IEnumerable<ValidationFailure> failures ) : this() {
        Errors = failures
            .GroupBy( e => e.PropertyName, e => e.ErrorMessage )
            .ToDictionary(
                failureGroup => failureGroup.Key,
                failureGroup => failureGroup.ToArray() );
    }
}
