using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.DocumentTypes.CreateDocumentType;

public record CreateDocumentTypeCommand(
    string Code,
    string Description
) : ICommand<CreateDocumentTypeResponse>;

public record CreateDocumentTypeResponse( Guid DocumentTypeId );
