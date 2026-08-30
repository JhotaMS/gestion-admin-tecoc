using gestionAdminTECOCApi.Domain.DomainService;
using gestionAdminTECOCApi.Domain.Ports;

namespace gestionAdminTECOCApi.Domain.ScheduledClasses;

[DomainService]
public class ScheduledClassService(
    IUnitOfWork unitOfWork
) {

    public async Task<Guid> CreateScheduledClassAsync(
        ScheduledClass scheduledClass,
        CancellationToken cancellationToken
    ) {
        ArgumentNullException.ThrowIfNull( scheduledClass );

        await unitOfWork.Repository<ScheduledClass>()
            .AddAsync( scheduledClass, cancellationToken );

        return scheduledClass.Id;
    }

    public async Task<bool> ExistsByScheduleAsync(
        DateOnly scheduledDate,
        TimeOnly scheduledTime,
        CancellationToken cancellationToken
    ) => await unitOfWork.Repository<ScheduledClass>()
        .Exitst(
            scheduledClass => scheduledClass.ScheduledDate == scheduledDate
                && scheduledClass.ScheduledTime == scheduledTime,
            cancellationToken
        );
}
