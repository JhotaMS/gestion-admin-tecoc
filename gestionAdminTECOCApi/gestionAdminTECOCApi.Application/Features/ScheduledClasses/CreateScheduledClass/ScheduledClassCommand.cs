using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.ScheduledClasses.CreateScheduledClass;

public record ScheduledClassCommand(
    string ScheduledDate
    , string ScheduledTime
    , string Topic
    , string CourseLevel
    ) : ICommand<ScheduledClassCommandResponse>;
