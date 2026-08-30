using MediatR;

namespace gestionAdminTECOCApi.Application.Messaging;

public interface INotifyHandler<TCommand> : INotificationHandler<TCommand>
where TCommand : INotify {

}
