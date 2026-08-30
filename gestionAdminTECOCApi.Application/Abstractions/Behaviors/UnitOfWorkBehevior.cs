using gestionAdminTECOCApi.Domain.Ports;
using MediatR;
using System.Transactions;

namespace gestionAdminTECOCApi.Application.Abstractions.Behaviors;

public sealed class UnitOfWorkBehevior<TRequest, TResponse>(
    IUnitOfWork unitOfWork
) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull {

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    ) {
        if (IsNotCommand())
            return await next();

        using TransactionScope transactionScope = new(
            TransactionScopeAsyncFlowOption.Enabled
        );

        TResponse? response = await next();

        try {

            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex) {

            throw;
        }
        transactionScope.Complete();
        return response;
    }

    private static bool IsNotCommand()
        => !typeof( TRequest ).Name.EndsWith( "Command" );
}