namespace gestionAdminTECOCApi.Domain.Users;

public static class DocumentTypeCodes {
    private static readonly Dictionary<string, DocumentType> _documentTypes =
        new( StringComparer.OrdinalIgnoreCase ) {
            ["CC"] = DocumentType.CedulaCiudadania,
            ["CE"] = DocumentType.CedulaExtranjeria,
            ["TI"] = DocumentType.TarjetaIdentidad,
            ["NIT"] = DocumentType.NumeroIdentificacionTributaria
        };

    public static IReadOnlyCollection<string> AllowedCodes => _documentTypes.Keys;

    public static bool IsAllowed( string? code )
        => !string.IsNullOrWhiteSpace( code )
        && _documentTypes.ContainsKey( code );

    public static bool TryParse( string? code, out DocumentType documentType ) {
        documentType = default;

        return !string.IsNullOrWhiteSpace( code )
            && _documentTypes.TryGetValue( code, out documentType );
    }

    public static string ToCode( DocumentType documentType )
        => _documentTypes
        .First( documentTypeCode => documentTypeCode.Value == documentType )
        .Key;
}
