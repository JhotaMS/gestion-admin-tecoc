using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.ScheduledClasses;

public class ScheduledClass : Entity<Guid> {
    private ScheduledClass(
        DateOnly scheduledDate,
        TimeOnly scheduledTime,
        string topic,
        string courseLevel
    ) : base( true ) {
        Id = Guid.NewGuid();
        ScheduledDate = scheduledDate;
        ScheduledTime = scheduledTime;
        Topic = topic;
        CourseLevel = courseLevel;
    }

    public DateOnly ScheduledDate { get; private set; }
    public TimeOnly ScheduledTime { get; private set; }
    public string Topic { get; private set; } = default!;
    public string CourseLevel { get; private set; } = default!;

    public static ScheduledClass Create(
        DateOnly scheduledDate,
        TimeOnly scheduledTime,
        string topic,
        string courseLevel
    ) => new(
        scheduledDate,
        scheduledTime,
        topic,
        courseLevel
    );
}
