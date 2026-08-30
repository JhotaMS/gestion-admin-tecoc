using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.DocumentTypes;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Application.Features.DocumentTypes.GetAllDocumentTypes;

public record GetAllDocumentTypesQuery : IQuery<GetAllDocumentTypesResponse>;

public record GetAllDocumentTypesResponse( IReadOnlyList<DocumentTypeDto> DocumentTypes );

public record DocumentTypeDto(
    Guid DocumentTypeId,
    string Code,
    string Description
);

internal sealed class GetAllDocumentTypesQueryHandler(
    IUnitOfWork unitOfWork
) : IQueryHandler<GetAllDocumentTypesQuery, GetAllDocumentTypesResponse> {

    public async Task<Result<GetAllDocumentTypesResponse>> Handle( GetAllDocumentTypesQuery request, CancellationToken cancellationToken ) {
        var repo = unitOfWork.Repository<DocumentTypeEntity>();
        var items = await repo.GetAllAsync( cancellationToken );

        var dtos = items
            .Select( d => new DocumentTypeDto( d.Id, d.Code, d.Description ) )
            .ToList();

        return Result.Success( new GetAllDocumentTypesResponse( dtos ) );
    }
}
