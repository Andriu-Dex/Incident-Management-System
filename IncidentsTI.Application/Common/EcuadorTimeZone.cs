namespace IncidentsTI.Application.Common;

/// <summary>
/// Helper class for Ecuador timezone (UTC-5, Ambato/Guayaquil)
/// Ecuador does not observe daylight saving time
/// </summary>
public static class EcuadorTimeZone
{
    /// <summary>
    /// Ecuador timezone ID (America/Guayaquil - UTC-5)
    /// </summary>
    private const string EcuadorTimeZoneId = "SA Pacific Standard Time"; // Windows timezone ID for UTC-5
    
    /// <summary>
    /// Gets the Ecuador timezone info
    /// </summary>
    public static TimeZoneInfo TimeZone => 
        TimeZoneInfo.FindSystemTimeZoneById(EcuadorTimeZoneId);
    
    /// <summary>
    /// Gets the current date and time in Ecuador (Ambato, Guayaquil)
    /// </summary>
    public static DateTime Now => 
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
    
    /// <summary>
    /// Gets the current date in Ecuador
    /// </summary>
    public static DateTime Today => Now.Date;
    
    /// <summary>
    /// Converts a UTC datetime to Ecuador time
    /// </summary>
    public static DateTime FromUtc(DateTime utcDateTime) =>
        TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZone);
    
    /// <summary>
    /// Converts an Ecuador datetime to UTC
    /// </summary>
    public static DateTime ToUtc(DateTime ecuadorDateTime) =>
        TimeZoneInfo.ConvertTimeToUtc(ecuadorDateTime, TimeZone);
    
    /// <summary>
    /// Gets a formatted date string in Ecuador timezone
    /// Format: dd/MM/yyyy HH:mm (24-hour format commonly used in Ecuador)
    /// </summary>
    public static string FormatDateTime(DateTime? dateTime) =>
        dateTime.HasValue 
            ? FromUtc(dateTime.Value.ToUniversalTime()).ToString("dd/MM/yyyy HH:mm")
            : "-";
    
    /// <summary>
    /// Gets a formatted date string in Ecuador timezone
    /// Format: dd/MM/yyyy
    /// </summary>
    public static string FormatDate(DateTime? dateTime) =>
        dateTime.HasValue 
            ? FromUtc(dateTime.Value.ToUniversalTime()).ToString("dd/MM/yyyy")
            : "-";
    
    /// <summary>
    /// Gets a formatted time string in Ecuador timezone
    /// Format: HH:mm
    /// </summary>
    public static string FormatTime(DateTime? dateTime) =>
        dateTime.HasValue 
            ? FromUtc(dateTime.Value.ToUniversalTime()).ToString("HH:mm")
            : "-";
}
