using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Domain.Tests;

[TestClass]
public class DocumentTypeCodesTests {

    [DataTestMethod]
    [DataRow( "CC", DocumentType.CedulaCiudadania )]
    [DataRow( "CE", DocumentType.CedulaExtranjeria )]
    [DataRow( "TI", DocumentType.TarjetaIdentidad )]
    [DataRow( "NIT", DocumentType.NumeroIdentificacionTributaria )]
    [DataRow( "cc", DocumentType.CedulaCiudadania )]
    [DataRow( "nit", DocumentType.NumeroIdentificacionTributaria )]
    public void TryParse_CodigoConfigurado_RetornaTrueYElTipoDeDocumento(
        string code
        , DocumentType expected
    ) {
        //Arrange

        //Act
        bool result = DocumentTypeCodes.TryParse( code, out DocumentType documentType );

        //Assert
        Assert.IsTrue( result );
        Assert.AreEqual( expected, documentType );
    }

    [DataTestMethod]
    [DataRow( null )]
    [DataRow( "" )]
    [DataRow( "   " )]
    [DataRow( "XX" )]
    [DataRow( "PASAPORTE" )]
    public void TryParse_CodigoNoConfigurado_RetornaFalse( string? code ) {
        //Arrange

        //Act
        bool result = DocumentTypeCodes.TryParse( code, out _ );

        //Assert
        Assert.IsFalse( result );
    }

    [DataTestMethod]
    [DataRow( "CC" )]
    [DataRow( "ce" )]
    public void IsAllowed_CodigoConfigurado_RetornaTrue( string code ) {
        //Arrange

        //Act
        bool result = DocumentTypeCodes.IsAllowed( code );

        //Assert
        Assert.IsTrue( result );
    }

    [DataTestMethod]
    [DataRow( null )]
    [DataRow( "" )]
    [DataRow( "   " )]
    [DataRow( "XX" )]
    public void IsAllowed_CodigoNoConfigurado_RetornaFalse( string? code ) {
        //Arrange

        //Act
        bool result = DocumentTypeCodes.IsAllowed( code );

        //Assert
        Assert.IsFalse( result );
    }

    [TestMethod]
    public void ToCode_TodosLosTiposConfigurados_RetornaElCodigoEsperado() {
        //Arrange

        //Act

        //Assert
        Assert.AreEqual( "CC", DocumentTypeCodes.ToCode( DocumentType.CedulaCiudadania ) );
        Assert.AreEqual( "CE", DocumentTypeCodes.ToCode( DocumentType.CedulaExtranjeria ) );
        Assert.AreEqual( "TI", DocumentTypeCodes.ToCode( DocumentType.TarjetaIdentidad ) );
        Assert.AreEqual( "NIT", DocumentTypeCodes.ToCode( DocumentType.NumeroIdentificacionTributaria ) );
    }

    [TestMethod]
    public void AllowedCodes_RetornaLosCodigosConfiguradosEnElSistema() {
        //Arrange

        //Act
        var result = DocumentTypeCodes.AllowedCodes;

        //Assert
        CollectionAssert.AreEquivalent(
            new[] { "CC", "CE", "TI", "NIT" }
            , result.ToArray()
        );
    }
}
