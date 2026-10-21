using System;
using PlatiniumUI.Fluent.DesignSystem.Globalization;
using Xunit;

namespace PlatiniumUI.Tests;

/// <summary>Covers digit normalization (the "silent 0" trap), parsing, and Rials formatting.</summary>
public class PersianFormatTests
{
    [Theory]
    [InlineData("۱۲۳", "123")]         // Persian digits
    [InlineData("٤٥٦", "456")]         // Arabic-Indic digits
    [InlineData("۲۵٫۵", "25.5")]        // Persian decimal separator ٫
    [InlineData("1,234", "1234")]       // ASCII thousands comma dropped
    [InlineData("۱۲,۳۴۵٫۶", "12345.6")] // mixed: Persian digits + separators
    [InlineData("-۷", "-7")]            // leading minus preserved, digits mapped
    public void NormalizeDigits_ToInvariantForm(string input, string expected) =>
        Assert.Equal(expected, PersianFormat.NormalizeDigits(input));

    [Fact]
    public void TryParseDecimal_PersianDigits_Succeeds_NotSilentZero()
    {
        Assert.True(PersianFormat.TryParseDecimal("۲۵٫۵", out var v));
        Assert.Equal(25.5m, v);
    }

    [Fact]
    public void TryParseDecimal_MilkVolume_KeepsFraction_NoRounding()
    {
        // The exact regression: 25.5 must not become 26.
        Assert.True(PersianFormat.TryParseDecimal("۲۵.۵", out var v));
        Assert.Equal(25.5m, v);
    }

    [Theory]
    [InlineData("abc", false)]
    [InlineData("", false)]
    public void TryParseDecimal_RejectsNonNumeric(string input, bool expected) =>
        Assert.Equal(expected, PersianFormat.TryParseDecimal(input, out _));

    [Fact]
    public void ToPersianDigits_MapsLatinOnly() =>
        Assert.Equal("۱۴۰۳/۰۱/۰۱", PersianFormat.ToPersianDigits("1403/01/01"));

    [Fact]
    public void ToLatinDigits_RoundTripsPersian() =>
        Assert.Equal("1234567890", PersianFormat.ToLatinDigits(PersianFormat.ToPersianDigits("1234567890")));

    [Fact]
    public void FormatRials_GroupsAndLocalizes() =>
        Assert.Equal("۱,۲۳۴,۵۶۷", PersianFormat.FormatRials(1234567m));

    [Fact]
    public void FaDate_UsesJalali_OnNowruzBoundary()
    {
        // 2024-03-20 (Tehran) is Farvardin 1, 1403 → ۱۴۰۳/۰۱/۰۱.
        var utc = new DateTime(2024, 3, 20, 0, 0, 0, DateTimeKind.Utc); // 03:30 Tehran
        Assert.Equal("۱۴۰۳/۰۱/۰۱", PersianFormat.FaDate(utc));
    }
}
