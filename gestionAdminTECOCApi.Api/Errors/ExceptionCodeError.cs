namespace gestionAdminTECOCApi.Api.Errors;

public record ExceptionCodeError : CodeError {
    public object? Details { get; set; }

    public ExceptionCodeError(
        int statusCode,
        string? message = null,
        object? details = null

    ) : base( statusCode, message ) {
        Details = details;
    }
}