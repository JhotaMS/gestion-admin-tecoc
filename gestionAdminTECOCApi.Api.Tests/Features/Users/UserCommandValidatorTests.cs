using FluentValidation.Results;
using gestionAdminTECOCApi.Application.Features.Users.CreateUser;

namespace gestionAdminTECOCApi.Api.Tests.Features.Users;

public class UserCommandValidatorTests {
    private readonly UserCommandValidator _validator = new();

    private static UserCommand ValidCommand() => new(
        "Juan Camilo Tamayo"
        , "CC"
        , "1094567890"
        , "jctamayo"
        , "juan.tamayo@example.com"
        , "Passw0rd!"
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
    public void Validate_NombreCompletoSinValor_RetornaErrorDelCampo( string? fullName ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { FullName = fullName! }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.FullName ) );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_TipoDocumentoSinValor_RetornaErrorDelCampo( string? documentType ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { DocumentType = documentType! }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.DocumentType ) );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_NumeroDocumentoSinValor_RetornaErrorDelCampo( string? documentNumber ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { DocumentNumber = documentNumber! }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.DocumentNumber ) );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_NombreUsuarioSinValor_RetornaErrorDelCampo( string? userName ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { UserName = userName! }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.UserName ) );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_EmailSinValor_RetornaErrorDelCampo( string? email ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { Email = email! }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.Email ) );
    }

    [Theory]
    [InlineData( "correo-invalido" )]
    [InlineData( "correo@" )]
    [InlineData( "@example.com" )]
    public void Validate_EmailFormatoInvalido_RetornaErrorDelCampo( string email ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { Email = email }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.Email ) );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Validate_ContrasenaSinValor_RetornaErrorDelCampo( string? password ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { Password = password! }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.Password ) );
    }

    [Fact]
    public void Validate_ContrasenaMuyCorta_RetornaErrorDelCampo() {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { Password = "1234567" }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.Password ) );
    }

    [Fact]
    public void Validate_TodosLosCamposVacios_RetornaUnErrorPorCadaCampo() {
        ValidationResult result = _validator.Validate(
            new UserCommand( string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty )
        );

        Assert.Equal( 6, result.Errors.Count );
        AssertErrorEnPropiedad( result, nameof( UserCommand.FullName ) );
        AssertErrorEnPropiedad( result, nameof( UserCommand.DocumentType ) );
        AssertErrorEnPropiedad( result, nameof( UserCommand.DocumentNumber ) );
        AssertErrorEnPropiedad( result, nameof( UserCommand.UserName ) );
        AssertErrorEnPropiedad( result, nameof( UserCommand.Email ) );
        AssertErrorEnPropiedad( result, nameof( UserCommand.Password ) );
    }

    [Theory]
    [InlineData( "XX" )]
    [InlineData( "PASAPORTE" )]
    [InlineData( "1" )]
    public void Validate_TipoDocumentoNoConfigurado_RetornaErrorDelCampo( string documentType ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { DocumentType = documentType }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.DocumentType ) );
    }

    [Theory]
    [InlineData( "CC" )]
    [InlineData( "CE" )]
    [InlineData( "TI" )]
    [InlineData( "NIT" )]
    [InlineData( "cc" )]
    public void Validate_TipoDocumentoConfigurado_NoRetornaErrores( string documentType ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { DocumentType = documentType }
        );

        Assert.True( result.IsValid );
    }

    [Theory]
    [InlineData( "abc123" )]
    [InlineData( "1.094.567.890" )]
    [InlineData( "1094-567890" )]
    [InlineData( "1094 567890" )]
    public void Validate_NumeroDocumentoNoNumerico_RetornaErrorDelCampo( string documentNumber ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { DocumentNumber = documentNumber }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.DocumentNumber ) );
    }

    [Theory]
    [InlineData( "1234" )]
    [InlineData( "1234567890123456" )]
    public void Validate_NumeroDocumentoFueraDeLongitud_RetornaErrorDelCampo( string documentNumber ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { DocumentNumber = documentNumber }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.DocumentNumber ) );
    }

    [Theory]
    [InlineData( "12345" )]
    [InlineData( "123456789012345" )]
    public void Validate_NumeroDocumentoEnLosLimites_NoRetornaErrores( string documentNumber ) {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { DocumentNumber = documentNumber }
        );

        Assert.True( result.IsValid );
    }

    [Fact]
    public void Validate_NombreCompletoExcedeLongitudMaxima_RetornaErrorDelCampo() {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { FullName = new string( 'a', 151 ) }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.FullName ) );
    }

    [Fact]
    public void Validate_NombreUsuarioExcedeLongitudMaxima_RetornaErrorDelCampo() {
        ValidationResult result = _validator.Validate(
            ValidCommand() with { UserName = new string( 'a', 51 ) }
        );

        AssertErrorEnPropiedad( result, nameof( UserCommand.UserName ) );
    }

    private static void AssertErrorEnPropiedad( ValidationResult result, string propertyName ) {
        Assert.False( result.IsValid );
        Assert.Contains(
            result.Errors,
            failure => failure.PropertyName == propertyName
        );
    }
}
