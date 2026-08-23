namespace gestionAdminTECOCApi.Application.Features.ScheduledClasses.CreateScheduledClass;

public record ScheduledClassCommandResponse(
    Guid Id
    , string ScheduledDate
    , string ScheduledTime
    , string Topic
    , string CourseLevel
);
