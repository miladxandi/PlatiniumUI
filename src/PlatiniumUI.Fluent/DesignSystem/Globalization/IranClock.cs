using System;

namespace PlatiniumUI.Fluent.DesignSystem.Globalization;

/// <summary>
/// Central, dependency-free resolver for the Iran (Tehran) wall clock. All Jalali
/// conversions in the app go through here so a UTC instant always becomes the same
/// Iranian calendar day the server would report (server uses Intl Asia/Tehran).
/// </summary>
public static class IranClock
{
    /// <summary>
    /// The Asia/Tehran time zone. Resolves the IANA id first (Android/mac/Linux),
    /// then the Windows id, then falls back to a fixed +03:30 offset — Iran
    /// abolished DST after 2022, so +03:30 is correct for all current dates.
    /// </summary>
    public static TimeZoneInfo Tehran { get; } = Resolve();

    private static TimeZoneInfo Resolve()
    {
        foreach (var id in new[] { "Asia/Tehran", "Iran Standard Time" })
        {
            try
            {
                if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var tz))
                    return tz;
            }
            catch (TimeZoneNotFoundException) { /* try next */ }
            catch (InvalidTimeZoneException) { /* try next */ }
        }

        return TimeZoneInfo.CreateCustomTimeZone(
            id: "Asia/Tehran (fixed)",
            baseUtcOffset: TimeSpan.FromHours(3.5),
            displayName: "Iran Standard Time",
            standardDisplayName: "Iran Standard Time");
    }

    /// <summary>Coerce any <see cref="DateTime"/> to a UTC instant (Unspecified is treated as UTC).</summary>
    public static DateTime AsUtc(DateTime dt) => dt.Kind switch
    {
        DateTimeKind.Utc => dt,
        DateTimeKind.Local => dt.ToUniversalTime(),
        _ => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
    };

    /// <summary>Convert a UTC instant to the Iran wall-clock <see cref="DateTime"/> (Kind=Unspecified).</summary>
    public static DateTime ToIranWallClock(DateTime utc) =>
        DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(AsUtc(utc), Tehran), DateTimeKind.Unspecified);

    /// <summary>Convert a UTC instant to an Iran offset-aware <see cref="DateTimeOffset"/>.</summary>
    public static DateTimeOffset ToIranOffset(DateTime utc) =>
        new(ToIranWallClock(utc), Tehran.BaseUtcOffset);
}
