using FluentValidation.TestHelper;
using gestionAdminTECOCApi.Application.Features.Loans.CreateImplementoPrestado;
using gestionAdminTECOCApi.Domain.Loans;

namespace gestionAdminTECOCApi.Api.Tests.Features.Loans;

public class CreateImplementoPrestadoCommandValidatorTests {
    private readonly CreateImplementoPrestadoCommandValidator _validator = new();

    private static CreateImplementoPrestadoCommand ValidCommand() => new(
        UserId: Guid.NewGuid(),
        ImplementoId: Guid.NewGuid(),
        TipoRevisionId: 1,
        EstadoTipo: EstadoTipoImplemento.Bueno,
        FechaInicio: DateTime.UtcNow,
        FechaFin: DateTime.UtcNow.AddDays( 2 ),
        Observacion: "Sin novedades"
    );

    [Fact]
    public void Validate_CommandValido_NoTieneErrores() {
        var result = _validator.TestValidate( ValidCommand() );
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_UserIdVacio_TieneError() {
        var command = ValidCommand() with { UserId = Guid.Empty };
        var result = _validator.TestValidate( command );
        result.ShouldHaveValidationErrorFor( c => c.UserId );
    }

    [Fact]
    public void Validate_ImplementoIdVacio_TieneError() {
        var command = ValidCommand() with { ImplementoId = Guid.Empty };
        var result = _validator.TestValidate( command );
        result.ShouldHaveValidationErrorFor( c => c.ImplementoId );
    }

    [Fact]
    public void Validate_TipoRevisionIdInvalido_TieneError() {
        var command = ValidCommand() with { TipoRevisionId = 0 };
        var result = _validator.TestValidate( command );
        result.ShouldHaveValidationErrorFor( c => c.TipoRevisionId );
    }

    [Fact]
    public void Validate_FechaFinMenorAFechaInicio_TieneError() {
        var command = ValidCommand() with {
            FechaInicio = DateTime.UtcNow.AddDays( 2 ),
            FechaFin = DateTime.UtcNow
        };
        var result = _validator.TestValidate( command );
        result.ShouldHaveValidationErrorFor( c => c.FechaFin );
    }

    [Fact]
    public void Validate_ObservacionMayorA500Caracteres_TieneError() {
        var command = ValidCommand() with {
            Observacion = new string( 'A', 501 )
        };
        var result = _validator.TestValidate( command );
        result.ShouldHaveValidationErrorFor( c => c.Observacion );
    }
}

