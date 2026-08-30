using gestionAdminTECOCApi.Api.Middlewares;
using gestionAdminTECOCApi.Application.Extensions;
using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Infrastructure.Extensions;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Extensions;

WebApplicationBuilder builder = WebApplication
    .CreateBuilder( args );

builder.Configuration
    .AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true )
    .AddJsonFile( $"appsettings.{builder.Environment.EnvironmentName}.json", optional: true )
    .AddEnvironmentVariables();

builder.Services
    .AddApplication( builder.Configuration )
    .AddDomainService()
    .AddInfrastructure()
    .AddInfrastructurePostgreSql( builder.Configuration );

builder.Services.AddTransient<IDispatch, Dispatch>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// Add CORS policy
builder.Services.AddCors( options => {
    options.AddPolicy( "AllowFrontend", policy => {
        policy
            .WithOrigins( "http://localhost:4200", "http://localhost:4201" )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    } );
} );

WebApplication app = builder.Build();

//app.UsePathBase( "/api" );
app.UseRouting();
app.UseCors( "AllowFrontend" );

app.UseSwagger();
app.UseSwaggerUI( options => {
    options.SwaggerEndpoint( "/swagger/v1/swagger.json", "gestionAdminTECOCApi.Api" );
    options.RoutePrefix = "swagger";
} );

app.UseMiddleware<ExceptionMiddleware>();
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync()
    .ConfigureAwait( default( bool ) );

public partial class Program { }
