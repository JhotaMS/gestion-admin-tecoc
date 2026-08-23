using FluentValidation.Results;
using gestionAdminTECOCApi.Application.Features.ScheduledClasses.CreateScheduledClass;
using gestionAdminTECOCApi.Domain.Helpers;

namespace gestionAdminTECOCApi.Api.Tests.Features.ScheduledClasses;

public class ScheduledClassCommandValidatorTests {
    private readonly ScheduledClassCommandValidator _validator = new();

    private static DateOnly Hoy()
        => DateOnly.FromDateTime( DateTime.Now.ZoneByIdPacificStandardTime() );

    private static string FechaEnDias( int dias )
        => Hoy().AddDays( dias ).ToString( "yyyy-MM-dd" );

    private static ScheduledClassCommand ValidCommand() => new(
        FechaEnDias( 1 )
        , "14:30"
        , "Ecuaciones diferenciales de primer orden"
        , "Unidad 3"
    );

    [Fact]
    public void Validate_ComandoValido_NoRetornaErrores() {
        ValidationResult result = _validator.Validate( ValidCommand() );

        Assert.True( result.IsValid );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_FechaSinValor_RetornaErrorDelCampo( string? scheduledDate ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { ScheduledDate = scheduledDate! }
        );

        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.ScheduledDate ) );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_HoraSinValor_RetornaErrorDelCampo( string? scheduledTime ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { ScheduledTime = scheduledTime! }
        );

        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.ScheduledTime ) );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_TemaSinValor_RetornaErrorDelCampo( string? topic ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { Topic = topic! }
        );

        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.Topic ) );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_NivelOUnidadSinValor_RetornaErrorDelCampo( string? courseLevel ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { CourseLevel = courseLevel! }
        );

        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.CourseLevel ) );
    }

    [Fact]
    public void Validate_TodosLosCamposVacios_RetornaUnErrorPorCadaCampo() {
        ValidationResult result = _validator.Validate(
            new ScheduledClassCommand( string.Empty, string.Empty, string.Empty, string.Empty )
        );

        Assert.Equal( 4, result.Errors.Count );
        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.ScheduledDate ) );
        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.ScheduledTime ) );
        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.Topic ) );
        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.CourseLevel ) );
    }

    [Theory]
    [InlineData( "01/09/2026" )]
    [InlineData( "2026-13-01" )]
    [InlineData( "2026-02-30" )]
    [InlineData( "2026/09/01" )]
    [InlineData( "manana" )]
    public void Validate_FechaConFormatoInvalido_RetornaErrorDelCampo( string scheduledDate ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { ScheduledDate = scheduledDate }
        );

        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.ScheduledDate ) );
    }

    [Fact]
    public void Validate_FechaAnteriorAHoy_RetornaErrorDelCampo() {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { ScheduledDate = FechaEnDias( -1 ) }
        );

        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.ScheduledDate ) );
    }

    [Fact]
    public void Validate_FechaDeHoy_NoRetornaErrores() {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { ScheduledDate = FechaEnDias( 0 ) }
        );

        Assert.True( result.IsValid );
    }

    [Theory]
    [InlineData( "25:00" )]
    [InlineData( "14:60" )]
    [InlineData( "2:30 pm" )]
    [InlineData( "14-30" )]
    [InlineData( "1430" )]
    public void Validate_HoraConFormatoInvalido_RetornaErrorDelCampo( string scheduledTime ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { ScheduledTime = scheduledTime }
        );

        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.ScheduledTime ) );
    }

    [Theory]
    [InlineData( "00:00" )]
    [InlineData( "07:05" )]
    [InlineData( "23:59" )]
    [InlineData( "14:30:00" )]
    public void Validate_HoraConFormatoValido_NoRetornaErrores( string scheduledTime ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { ScheduledTime = scheduledTime }
        );

        Assert.True( result.IsValid );
    }

    [Fact]
    public void Validate_TemaExcedeLongitudMaxima_RetornaErrorDelCampo() {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { Topic = new string( 'a', 201 ) }
        );

        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.Topic ) );
    }

    [Fact]
    public void Validate_NivelOUnidadExcedeLongitudMaxima_RetornaErrorDelCampo() {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { CourseLevel = new string( 'a', 101 ) }
        );

        AssertErrorEnPropiedad( result, nameof( ScheduledClassCommand.CourseLevel ) );
    }

    private static void AssertErrorEnPropiedad( ValidationResult result, string propertyName ) {
        Assert.False( result.IsValid );
        Assert.Contains(
            result.Errors,
            failure => failure.PropertyName == propertyName
        );
    }
}
