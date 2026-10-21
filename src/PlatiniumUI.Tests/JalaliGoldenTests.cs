using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using PlatiniumUI.Fluent.DesignSystem.Globalization;
using Xunit;

namespace PlatiniumUI.Tests;

/// <summary>
/// Arbiters the .NET Jalali port against golden fixtures produced by the web app's
/// own Intl('en-u-ca-persian', Asia/Tehran) implementation (see Fixtures/*.mjs).
/// If ICU and <see cref="PersianCalendar"/> ever diverge, this test is the authority.
/// </summary>
public class JalaliGoldenTests
{
    private sealed record JalaliParts(int year, int month, int day);
    private sealed record PartFixture(string gregorianUtcIso, JalaliParts jalali);
    private sealed record MonthStartFixture(int jy, int jm, string gregorianStartUtc);
    private sealed record GoldenDoc(
        [property: JsonPropertyName("parts")] IReadOnlyList<PartFixture> Parts,
        [property: JsonPropertyName("monthStarts")] IReadOnlyList<MonthStartFixture> MonthStarts);

    private static GoldenDoc Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "jalali-golden.json");
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<GoldenDoc>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    [Fact]
    public void JalaliParts_MatchWebIntl_ForAllInstants()
    {
        var doc = Load();
        foreach (var f in doc.Parts)
        {
            var utc = DateTime.Parse(f.gregorianUtcIso, CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
            var actual = Jalali.FromUtc(utc);
            Assert.Equal(f.jalali.year, actual.Year);
            Assert.Equal(f.jalali.month, actual.Month);
            Assert.Equal(f.jalali.day, actual.Day);
        }
    }

    [Fact]
    public void MonthStartUtc_MatchWebDayWalk_ForEveryMonth()
    {
        var doc = Load();
        foreach (var f in doc.MonthStarts)
        {
            var expected = DateTime.Parse(f.gregorianStartUtc, CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal).Date;
            var actual = Jalali.MonthStartUtc(f.jy, f.jm).Date;
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public void MonthRange_IsMonotonic_AndSpansExactlyOneMonth()
    {
        var (start, end) = Jalali.MonthRangeUtc(1403, 1);
        Assert.True(end > start);
        Assert.True((end - start).TotalDays is >= 30 and <= 31);
        // Month 12 (Esfand) end must be Farvardin 1 of the next year.
        var (dStart, dEnd) = Jalali.MonthRangeUtc(1403, 12);
        Assert.Equal(Jalali.MonthStartUtc(1404, 1), dEnd);
    }
}
