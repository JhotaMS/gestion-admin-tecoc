using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.DocumentTypes;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.DocumentTypes.UpdateDocumentType;

public record UpdateDocumentTypeCommand(
    Guid DocumentTypeId,
    string Code,
    string Description
) : ICommand;

internal sealed class UpdateDocumentTypeCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateDocumentTypeCommand> {

    public async Task<Result> Handle( UpdateDocumentTypeCommand request, CancellationToken cancellationToken ) {
        var repo = unitOfWork.Repository<DocumentTypeEntity>();
        var items = await repo.GetAsync( d => d.Id == request.DocumentTypeId, cancellationToken: cancellationToken );
        var entity = items.FirstOrDefault();
        if (entity is null) {
            return Result.Failure( new Error( "DocumentType.NotFound", "Tipo de documento no encontrado" ) );
        }

        entity.Update( request.Code, request.Description );
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
