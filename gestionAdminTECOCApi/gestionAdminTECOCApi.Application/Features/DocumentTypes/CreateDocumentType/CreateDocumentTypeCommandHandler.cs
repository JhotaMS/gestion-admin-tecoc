using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.DocumentTypes;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.DocumentTypes.CreateDocumentType;

internal sealed class CreateDocumentTypeCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateDocumentTypeCommand, CreateDocumentTypeResponse> {

    public async Task<Result<CreateDocumentTypeResponse>> Handle( CreateDocumentTypeCommand request, CancellationToken cancellationToken ) {
        var repo = unitOfWork.Repository<DocumentTypeEntity>();
        var existing = await repo.GetAsync( d => d.Code == request.Code, cancellationToken: cancellationToken );
        if (existing.Any()) {
            return Result.Failure<CreateDocumentTypeResponse>( new Error( "DocumentType.DuplicateCode", "Ya existe un tipo de documento con ese código" ) );
        }

        var entity = DocumentTypeEntity.Create( request.Code, request.Description );
        await repo.AddAsync( entity, cancellationToken );
        await unitOfWork.SaveChangesAsync();

        return Result.Success( new CreateDocumentTypeResponse( entity.Id ) );
    }
}
