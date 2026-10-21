using System;
using System.Globalization;

namespace PlatiniumUI.Fluent.DesignSystem.Globalization;

/// <summary>A plain Persian-calendar date (year 13xx/14xx, month 1..12, day 1..31).</summary>
public readonly record struct JalaliDate(int Year, int Month, int Day)
{
    public override string ToString() => $"{Year}/{Month:00}/{Day:00}";
}

/// <summary>
/// Jalali (Persian calendar) engine for the OK Pars app. Semantics mirror the web
/// app's <c>server/utils/jalali.ts</c>: an instant is converted to the Asia/Tehran
/// wall clock, then to Persian calendar fields. Month-start lookup uses the same
/// bounded day-walk the server uses so the client and server agree on accounting
/// periods. <see cref="PersianCalendar"/> is used for field extraction; golden
/// fixtures in PlatiniumUI.Tests arbitrate any divergence.
/// </summary>
public static class Jalali
{
    /// <summary>The 12 Persian month names, index 0 = Farvardin.</summary>
    public static readonly string[] MonthNames =
    {
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند",
    };

    private static readonly PersianCalendar Pc = new();
    private static readonly TimeSpan Day = TimeSpan.FromDays(1);

    /// <summary>Persian month name for a 1-based month index (empty when out of range).</summary>
    public static string MonthName(int month) =>
        month is >= 1 and <= 12 ? MonthNames[month - 1] : string.Empty;

    /// <summary>Sortable key so two Jalali periods compare without label arithmetic.</summary>
    public static int Key(JalaliDate d) => d.Year * 12 + d.Month;

    /// <summary>Jalali fields of a UTC instant, evaluated on the Iran wall clock.</summary>
    public static JalaliDate FromUtc(DateTime utc)
    {
        var wall = IranClock.ToIranWallClock(utc);
        return new JalaliDate(Pc.GetYear(wall), Pc.GetMonth(wall), Pc.GetDayOfMonth(wall));
    }

    /// <summary>Jalali fields of an Iran wall-clock <see cref="DateTime"/> (Kind ignored).</summary>
    public static JalaliDate FromIranWallClock(DateTime wall) =>
        new(Pc.GetYear(wall), Pc.GetMonth(wall), Pc.GetDayOfMonth(wall));

    /// <summary>
    /// Gregorian (UTC midnight) start of a Jalali month, found by the same bounded
    /// day-walk as the server so periods line up exactly.
    /// </summary>
    public static DateTime MonthStartUtc(int jy, int jm)
    {
        // Farvardin 1 of jy ≈ 20 March of (jy + 621); nudge by whole 30-day months.
        var g = new DateTime(jy + 621, 3, 20, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromDays((jm - 1) * 30.0);
        var p = FromUtc(g);
        // Monotonic period key (base 13 keeps months from bleeding into the next year).
        static int Order(JalaliDate d) => d.Year * 13 + d.Month;
        var target = jy * 13 + jm;

        var guard = 0;
        while (Order(p) < target && guard++ < 500) { g += Day; p = FromUtc(g); }
        guard = 0;
        while ((p.Day != 1 || Order(p) > target) && guard++ < 40) { g -= Day; p = FromUtc(g); }

        return DateTime.SpecifyKind(g.Date, DateTimeKind.Utc);
    }

    /// <summary>Inclusive-exclusive Gregorian UTC range <c>[start, end)</c> covering a Jalali month.</summary>
    public static (DateTime Start, DateTime End) MonthRangeUtc(int jy, int jm)
    {
        var start = MonthStartUtc(jy, jm);
        var end = jm == 12 ? MonthStartUtc(jy + 1, 1) : MonthStartUtc(jy, jm + 1);
        return (start, end);
    }

    /// <summary>Number of days in a Jalali month (handles Esfand 29 vs 30 on leap years).</summary>
    public static int DaysInMonth(int jy, int jm) => Pc.GetDaysInMonth(jy, jm);

    /// <summary>Whether a Jalali year is a leap year (Esfand has 30 days).</summary>
    public static bool IsLeapYear(int jy) => Pc.GetDaysInMonth(jy, 12) == 30;

    /// <summary>Convert Jalali fields to a Gregorian <see cref="DateTime"/> (Unspecified kind, midnight).</summary>
    public static DateTime ToGregorian(JalaliDate d) =>
        Pc.ToDateTime(d.Year, d.Month, d.Day, 0, 0, 0, 0);
}
