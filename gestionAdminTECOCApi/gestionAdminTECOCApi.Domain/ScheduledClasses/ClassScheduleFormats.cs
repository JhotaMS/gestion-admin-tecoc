using System.Globalization;

namespace gestionAdminTECOCApi.Domain.ScheduledClasses;

public static class ClassScheduleFormats {
    public const string DateFormat = "yyyy-MM-dd";
    public const string TimeFormat = "HH:mm";

    private static readonly string[] _timeFormats = ["HH:mm", "HH:mm:ss"];

    public static bool TryParseDate( string? scheduledDate, out DateOnly date )
        => DateOnly.TryParseExact(
            scheduledDate?.Trim(),
            DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date
        );

    public static bool TryParseTime( string? scheduledTime, out TimeOnly time )
        => TimeOnly.TryParseExact(
            scheduledTime?.Trim(),
            _timeFormats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out time
        );

    public static string ToDateCode( DateOnly date )
        => date.ToString( DateFormat, CultureInfo.InvariantCulture );

    public static string ToTimeCode( TimeOnly time )
        => time.ToString( TimeFormat, CultureInfo.InvariantCulture );
}
