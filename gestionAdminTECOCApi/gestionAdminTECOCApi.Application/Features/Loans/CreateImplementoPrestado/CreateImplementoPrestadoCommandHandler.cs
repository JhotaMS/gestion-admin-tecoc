using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Loans;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;

namespace gestionAdminTECOCApi.Application.Features.Loans.CreateImplementoPrestado;

internal sealed class CreateImplementoPrestadoCommandHandler(
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateImplementoPrestadoCommand, CreateImplementoPrestadoResponse> {

    public async Task<Result<CreateImplementoPrestadoResponse>> Handle(
        CreateImplementoPrestadoCommand request,
        CancellationToken cancellationToken
    ) {
        if (request.FechaFin < request.FechaInicio) {
            return Result.Failure<CreateImplementoPrestadoResponse>(
                ImplementoPrestadoErrors.InvalidDateRange
            );
        }

        var userRepo = unitOfWork.Repository<User>();
        bool userExists = await userRepo.Exitst(
            u => u.Id == request.UserId,
            cancellationToken
        );

        if (!userExists) {
            return Result.Failure<CreateImplementoPrestadoResponse>(
                ImplementoPrestadoErrors.UserNotFound
            );
        }

        var entity = ImplementoPrestado.Create(
            request.UserId,
            request.ImplementoId,
            request.TipoRevisionId,
            request.EstadoTipo,
            request.FechaInicio,
            request.FechaFin,
            request.Observacion?.Trim()
        );

        var repo = unitOfWork.Repository<ImplementoPrestado>();
        await repo.AddAsync( entity, cancellationToken );
        await unitOfWork.SaveChangesAsync();

        return Result.Success( new CreateImplementoPrestadoResponse(
            entity.Id,
            entity.UserId,
            entity.ImplementoId,
            entity.TipoRevisionId,
            entity.EstadoTipo.ToString(),
            entity.FechaInicio,
            entity.FechaFin,
            entity.Observacion
        ) );
    }
}

