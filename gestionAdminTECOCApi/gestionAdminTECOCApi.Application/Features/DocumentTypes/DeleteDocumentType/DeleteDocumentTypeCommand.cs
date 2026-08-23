using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.DocumentTypes;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.DocumentTypes.DeleteDocumentType;

public record DeleteDocumentTypeCommand(
    Guid DocumentTypeId
) : ICommand;

internal sealed class DeleteDocumentTypeCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<DeleteDocumentTypeCommand> {

    public async Task<Result> Handle( DeleteDocumentTypeCommand request, CancellationToken cancellationToken ) {
        var repo = unitOfWork.Repository<DocumentTypeEntity>();
        var items = await repo.GetAsync( d => d.Id == request.DocumentTypeId, cancellationToken: cancellationToken );
        var entity = items.FirstOrDefault();
        if (entity is null) {
            return Result.Failure( new Error( "DocumentType.NotFound", "Tipo de documento no encontrado" ) );
        }

        repo.Delete( entity );
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
