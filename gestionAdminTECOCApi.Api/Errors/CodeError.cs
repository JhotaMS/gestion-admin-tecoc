namespace gestionAdminTECOCApi.Api.Errors;

public record CodeError {
    public int StatusCode { get; set; }

    public string? Message { get; set; }

    public CodeError( int statusCode, string? message = null ) {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessageStatusCode( statusCode );
    }

    private static string GetDefaultMessageStatusCode( int statusCode ) => statusCode switch {
        400 => "El Request enviado tiene errores",
        401 => "No tienes authorizacion para este recurso",
        404 => "No se encontro el recurso solicitado",
        500 => "Se producieron errores en el servidor",
        _ => string.Empty
    };
}
