using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace gestionAdminTECOCApi.Api.Middlewares;

public class ExceptionMiddleware(
    RequestDelegate _next
    , ILogger<ExceptionMiddleware> _logger
    , IHostEnvironment _environment
) {

    public async Task InvokeAsync( HttpContext context ) {
        try {
            await _next( context );
        }
        catch (Exception ex) {
            _logger.LogError( "{@ex} {@message}", ex, ex.Message );
            context.Response.ContentType = "application/json";
            int statusCode;
            var result = string.Empty;

            switch (ex) {
                case NotFoundApplicationException _:
                    statusCode = (int)HttpStatusCode.NotFound;
                    break;

                case NotFoundException notFoundException:
                    statusCode = notFoundException.StatusCode;
                    break;

                case ValidationApplicationException validationException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(
                        new ExceptionCodeError(
                            statusCode,
                            ex.Message,
                            validationException.Errors
                        )
                    );
                    break;

                case BadRequestApplicationException _:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case ErrorInternalApplicationException _:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    break;

                default: {
                        var statusCodeProperty = ex.GetType().GetProperty( "StatusCode" );
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        result = JsonConvert.SerializeObject(
                            new ExceptionCodeError(
                                statusCode,
                                ex.Message,
                                new ErrorInternalApplicationException( ex.StackTrace )
                            )
                        );
                        break;
                    }
            }

            if (string.IsNullOrEmpty( result )) {
                if (_environment.IsDevelopment()) {
                    result = JsonConvert.SerializeObject(
                        new ExceptionCodeError(
                            statusCode,
                            ex.Message,
                            ex.StackTrace ) );
                }
                else {
                    result = JsonConvert.SerializeObject(
                        new ExceptionCodeError(
                            statusCode,
                            ex.Message ) );
                }
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync( result );
        }
    }
}