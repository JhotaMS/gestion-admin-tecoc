using gestionAdminTECOCApi.Domain.Abstractions;
using MediatR;

namespace gestionAdminTECOCApi.Application.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand {

}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand {

}