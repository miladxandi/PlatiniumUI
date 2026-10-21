using System;
using System.Globalization;
using System.Text;

namespace PlatiniumUI.Fluent.DesignSystem.Globalization;

/// <summary>
/// Persian display + input-normalization helpers. Dependency-free (BCL only) so the
/// same code runs in the app, in PlatiniumUI controls, and in unit tests.
/// </summary>
public static class PersianFormat
{
    private const char FaZero = '۰';   // ۰ Persian digits U+06F0..U+06F9
    private const char ArZero = '٠';   // ٠ Arabic-Indic digits U+0660..U+0669

    /// <summary>Map ASCII/Latin digits in <paramref name="input"/> to Persian digits.</summary>
    public static string ToPersianDigits(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
            sb.Append(c is >= '0' and <= '9' ? (char)(FaZero + (c - '0')) : c);
        return sb.ToString();
    }

    /// <summary>Map Persian and Arabic-Indic digits back to ASCII 0-9.</summary>
    public static string ToLatinDigits(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (c >= FaZero && c <= FaZero + 9) sb.Append((char)('0' + (c - FaZero)));
            else if (c >= ArZero && c <= ArZero + 9) sb.Append((char)('0' + (c - ArZero)));
            else sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Normalize a user-typed numeric string to an invariant-parseable form:
    /// Persian/Arabic digits → ASCII, Persian decimal '٫' (U+066B) and Arabic '،'
    /// → '.', strip Persian thousands separator '٬' (U+066C) and commas, trim spaces.
    /// Critical because fa-locale soft keyboards emit Persian digits; without this
    /// <c>decimal.TryParse(…, InvariantCulture)</c> silently yields 0.
    /// </summary>
    public static string NormalizeDigits(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (c >= FaZero && c <= FaZero + 9) sb.Append((char)('0' + (c - FaZero)));
            else if (c >= ArZero && c <= ArZero + 9) sb.Append((char)('0' + (c - ArZero)));
            else if (c is >= '0' and <= '9') sb.Append(c);
            else if (c is '٫' or '،' or '.') sb.Append('.');        // decimal separators → '.'
            else if (c is '٬' or ',') { /* thousands separator: drop */ }
            else if (c is '−' or '–' or '-') sb.Append('-');        // various dashes → '-'
            else if (!char.IsWhiteSpace(c)) sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>Parse a Persian/Arabic/Latin numeric string with invariant semantics.</summary>
    public static bool TryParseDecimal(string? input, out decimal value) =>
        decimal.TryParse(NormalizeDigits(input), NumberStyles.Number, CultureInfo.InvariantCulture, out value);

    /// <summary>Format an amount in Rials with thousands separators and Persian digits.</summary>
    public static string FormatRials(decimal amount) =>
        ToPersianDigits(amount.ToString("N0", CultureInfo.InvariantCulture));

    /// <summary>Persian date label (Jalali) for a UTC instant, e.g. ۱۴۰۳/۰۱/۰۱.</summary>
    public static string FaDate(DateTime utc)
    {
        var d = Jalali.FromUtc(utc);
        return ToPersianDigits($"{d.Year}/{d.Month:00}/{d.Day:00}");
    }

    /// <summary>Persian 24-hour time label (Iran wall clock), e.g. ۰۳:۳۰.</summary>
    public static string FaTime(DateTime utc)
    {
        var wall = IranClock.ToIranWallClock(utc);
        return ToPersianDigits(wall.ToString("HH:mm", CultureInfo.InvariantCulture));
    }

    /// <summary>Combined Jalali date + Iran time label.</summary>
    public static string FaDateTime(DateTime utc) => $"{FaDate(utc)} {FaTime(utc)}";
}
