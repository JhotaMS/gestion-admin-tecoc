using gestionAdminTECOCApi.Application.Messaging;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.ScheduledClasses;

namespace gestionAdminTECOCApi.Application.Features.ScheduledClasses.CreateScheduledClass;

internal sealed class ScheduledClassCommandHandler(
    ScheduledClassService scheduledClassService
) : ICommandHandler<ScheduledClassCommand, ScheduledClassCommandResponse> {

    public async Task<Result<ScheduledClassCommandResponse>> Handle( ScheduledClassCommand request
        , CancellationToken cancellationToken
    ) {
        IReadOnlyList<string> validationMessages = ScheduledClassCommandRules.Validate( request );

        if (validationMessages.Count != 0) {
            return Result.Failure<ScheduledClassCommandResponse>(
                ScheduledClassErrors.ValidationFailed( validationMessages )
            );
        }

        ClassScheduleFormats.TryParseDate( request.ScheduledDate, out DateOnly scheduledDate );
        ClassScheduleFormats.TryParseTime( request.ScheduledTime, out TimeOnly scheduledTime );

        bool scheduleAlreadyTaken = await scheduledClassService
            .ExistsByScheduleAsync(
                scheduledDate
                , scheduledTime
                , cancellationToken
            );

        if (scheduleAlreadyTaken) {
            return Result.Failure<ScheduledClassCommandResponse>(
                ScheduledClassErrors.ScheduleAlreadyTaken(
                    ClassScheduleFormats.ToDateCode( scheduledDate )
                    , ClassScheduleFormats.ToTimeCode( scheduledTime )
                )
            );
        }

        ScheduledClass scheduledClass = ScheduledClass.Create(
            scheduledDate
            , scheduledTime
            , request.Topic.Trim()
            , request.CourseLevel.Trim()
        );

        Guid id = await scheduledClassService
            .CreateScheduledClassAsync( scheduledClass, cancellationToken );

        return new ScheduledClassCommandResponse(
            id
            , ClassScheduleFormats.ToDateCode( scheduledClass.ScheduledDate )
            , ClassScheduleFormats.ToTimeCode( scheduledClass.ScheduledTime )
            , scheduledClass.Topic
            , scheduledClass.CourseLevel
        );
    }
}
