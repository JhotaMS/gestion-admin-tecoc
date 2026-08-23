using gestionAdminTECOCApi.Application.Messaging;
using System.ComponentModel.DataAnnotations;

namespace gestionAdminTECOCApi.Application.Features.ScheduledClasses.CreateScheduledClass;

public record ScheduledClassCommand(
    [Required] string ScheduledDate
    , [Required] string ScheduledTime
    , [Required] string Topic
    , [Required] string CourseLevel
    ) : ICommand<ScheduledClassCommandResponse>;
