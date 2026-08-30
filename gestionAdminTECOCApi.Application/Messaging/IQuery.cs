using gestionAdminTECOCApi.Domain.Abstractions;
using MediatR;

namespace gestionAdminTECOCApi.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> {

}