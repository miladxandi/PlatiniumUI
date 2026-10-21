using Xunit;

namespace PlatiniumUI.Tests;

/// <summary>
/// Baseline smoke test proving the test harness restores and runs offline.
/// Real globalization contract tests (Jalali golden fixtures, PersianFormat) are added in Milestone 2.
/// </summary>
public class SmokeTests
{
    [Fact]
    public void Harness_Runs()
    {
        Assert.True(true);
    }
}
