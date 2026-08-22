using gestionAdminTECOCApi.Domain.Abstractions;
using MediatR;

namespace gestionAdminTECOCApi.Application.Messaging;

public interface IQueryHandler<TQuery, TResponse>
: IRequestHandler<TQuery, Result<TResponse>>
where TQuery : IQuery<TResponse> {

}
