using DeskBoxWhite.Services;

namespace DeskBoxWhite.Tests;

public sealed class StartupLaunchPolicyTests
{
    [Theory]
    [InlineData("--startup")]
    [InlineData("--STARTUP")]
    [InlineData("\"--startup\"")]
    public void IsStartupLaunch_AcceptsProcessArgument(string argument)
    {
        Assert.True(StartupLaunchPolicy.IsStartupLaunch(["DeskBoxWhite.exe", argument]));
    }

    [Fact]
    public void IsStartupLaunch_AcceptsActivationArguments()
    {
        Assert.True(StartupLaunchPolicy.IsStartupLaunch(
            ["DeskBoxWhite.exe"],
            "--some-argument --startup"));
    }

    [Fact]
    public void IsStartupLaunch_AcceptsStartupTaskActivation()
    {
        Assert.True(StartupLaunchPolicy.IsStartupLaunch(
            ["DeskBoxWhite.exe"],
            isStartupTaskActivation: true));
    }

    [Fact]
    public void IsStartupLaunch_AcceptsScheduledTaskCommandLine()
    {
        Assert.True(StartupLaunchPolicy.IsStartupLaunch(
            ["DeskBoxWhite.exe", "--startup", "--startup-source=scheduled-task"]));
    }

    [Fact]
    public void IsStartupLaunch_RejectsOrdinaryLaunch()
    {
        Assert.False(StartupLaunchPolicy.IsStartupLaunch(
            ["DeskBoxWhite.exe"],
            "--some-argument"));
    }
}
