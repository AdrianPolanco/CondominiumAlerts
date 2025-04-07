namespace CondominiumAlerts.Features.Features;

public static class TimeHelper
{
    public static (DateTime start, DateTime end) ConvertToUtc(DateTime start, DateTime end)
    {
        TimeZoneInfo rdTimeZone;

        if (OperatingSystem.IsWindows())
        {
            rdTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
        }
        else
        {
            rdTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Santo_Domingo");
        }

        // Convertir correctamente a UTC
        var startDateTimeUtc = TimeZoneInfo.ConvertTimeToUtc(start, rdTimeZone);
        var endDateTimeUtc = TimeZoneInfo.ConvertTimeToUtc(end, rdTimeZone);
        
        return (startDateTimeUtc, endDateTimeUtc);
    }
}