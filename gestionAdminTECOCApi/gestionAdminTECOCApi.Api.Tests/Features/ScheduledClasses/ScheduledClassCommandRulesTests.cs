using gestionAdminTECOCApi.Application.Features.ScheduledClasses.CreateScheduledClass;
using gestionAdminTECOCApi.Domain.Helpers;

namespace gestionAdminTECOCApi.Api.Tests.Features.ScheduledClasses;

public class ScheduledClassCommandRulesTests {

    private static string FechaEnDias( int dias )
        => DateOnly.FromDateTime( DateTime.Now.ZoneByIdPacificStandardTime() )
        .AddDays( dias )
        .ToString( "yyyy-MM-dd" );

    private static ScheduledClassCommand ValidCommand() => new(
        FechaEnDias( 1 )
        , "14:30"
        , "Ecuaciones diferenciales de primer orden"
        , "Unidad 3"
    );

    [Fact]
    public void Validate_ComandoValido_NoRetornaMensajes() {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate( ValidCommand() );

        Assert.Empty( messages );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_FechaSinValor_RetornaMensajeDeObligatoriedad( string? scheduledDate ) {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate(
            ValidCommand() with { ScheduledDate = scheduledDate! }
        );

        Assert.Contains( "La fecha de la clase es obligatoria", messages );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_HoraSinValor_RetornaMensajeDeObligatoriedad( string? scheduledTime ) {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate(
            ValidCommand() with { ScheduledTime = scheduledTime! }
        );

        Assert.Contains( "La hora de la clase es obligatoria", messages );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_TemaSinValor_RetornaMensajeDeObligatoriedad( string? topic ) {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate(
            ValidCommand() with { Topic = topic! }
        );

        Assert.Contains( "El tema de la clase es obligatorio", messages );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_NivelOUnidadSinValor_RetornaMensajeDeObligatoriedad( string? courseLevel ) {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate(
            ValidCommand() with { CourseLevel = courseLevel! }
        );

        Assert.Contains( "El nivel o unidad del curso es obligatorio", messages );
    }

    [Fact]
    public void Validate_TodosLosCamposVacios_RetornaUnMensajePorCadaCampo() {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate(
            new ScheduledClassCommand( string.Empty, string.Empty, string.Empty, string.Empty )
        );

        Assert.Equal( 4, messages.Count );
    }

    [Theory]
    [InlineData( "01/09/2026" )]
    [InlineData( "2026-13-01" )]
    [InlineData( "2026-02-30" )]
    [InlineData( "2026/09/01" )]
    [InlineData( "manana" )]
    public void Validate_FechaConFormatoInvalido_RetornaMensajeDeFormato( string scheduledDate ) {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate(
            ValidCommand() with { ScheduledDate = scheduledDate }
        );

        Assert.Contains( messages, message => message.Contains( "yyyy-MM-dd", StringComparison.Ordinal ) );
    }

    [Theory]
    [InlineData( -365 )]
    [InlineData( -1 )]
    [InlineData( 0 )]
    [InlineData( 365 )]
    public void Validate_CualquierFechaConFormatoValido_NoRetornaMensajes( int dias ) {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate(
            ValidCommand() with { ScheduledDate = FechaEnDias( dias ) }
        );

        Assert.Empty( messages );
    }

    [Theory]
    [InlineData( "25:00" )]
    [InlineData( "14:60" )]
    [InlineData( "2:30 pm" )]
    [InlineData( "14-30" )]
    [InlineData( "1430" )]
    public void Validate_HoraConFormatoInvalido_RetornaMensajeDeFormato( string scheduledTime ) {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate(
            ValidCommand() with { ScheduledTime = scheduledTime }
        );

        Assert.Contains( messages, message => message.Contains( "HH:mm", StringComparison.Ordinal ) );
    }

    [Theory]
    [InlineData( "00:00" )]
    [InlineData( "07:05" )]
    [InlineData( "23:59" )]
    [InlineData( "14:30:00" )]
    public void Validate_HoraConFormatoValido_NoRetornaMensajes( string scheduledTime ) {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate(
            ValidCommand() with { ScheduledTime = scheduledTime }
        );

        Assert.Empty( messages );
    }

    [Fact]
    public void Validate_TemaExcedeLongitudMaxima_RetornaMensajeDeLongitud() {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate(
            ValidCommand() with { Topic = new string( 'a', 201 ) }
        );

        Assert.Contains( "El tema de la clase no puede superar los 200 caracteres", messages );
    }

    [Fact]
    public void Validate_NivelOUnidadExcedeLongitudMaxima_RetornaMensajeDeLongitud() {
        IReadOnlyList<string> messages = ScheduledClassCommandRules.Validate(
            ValidCommand() with { CourseLevel = new string( 'a', 101 ) }
        );

        Assert.Contains( "El nivel o unidad del curso no puede superar los 100 caracteres", messages );
    }
}
