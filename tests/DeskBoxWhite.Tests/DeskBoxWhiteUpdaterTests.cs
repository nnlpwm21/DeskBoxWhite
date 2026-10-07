namespace DeskBoxWhite.Tests;

public sealed class DeskBoxWhiteUpdaterTests
{
    [Fact]
    public void InstallerArguments_ShowProgressAndLockTheExistingDirectory()
    {
        const string InstallDirectory = @"D:\Apps\DeskBoxWhite";

        IReadOnlyList<string> arguments = DeskBoxWhite.Updater.Program.BuildInstallerArguments(
            InstallDirectory,
            silent: true,
            DeskBoxWhite.Updater.DirectInstallScope.CurrentUser);

        Assert.Contains($"/DIR={InstallDirectory}", arguments);
        Assert.Contains("/CURRENTUSER", arguments);
        Assert.DoesNotContain("/ALLUSERS", arguments);
        Assert.Contains("/SILENT", arguments);
        Assert.Contains("/SP-", arguments);
        Assert.Contains("/NORESTART", arguments);
        Assert.Contains("/FORCECLOSEAPPLICATIONS", arguments);
        Assert.DoesNotContain("/VERYSILENT", arguments);
        Assert.DoesNotContain("/SUPPRESSMSGBOXES", arguments);
    }

    [Fact]
    public void MachineWideInstallerArguments_PreserveAllUsersScope()
    {
        IReadOnlyList<string> arguments = DeskBoxWhite.Updater.Program.BuildInstallerArguments(
            @"C:\Program Files\DeskBoxWhite",
            silent: true,
            DeskBoxWhite.Updater.DirectInstallScope.AllUsers);

        Assert.Contains("/ALLUSERS", arguments);
        Assert.DoesNotContain("/CURRENTUSER", arguments);
    }

    [Fact]
    public void ProgramFilesInstallWithoutRegistration_FallsBackToAllUsersScope()
    {
        string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        Assert.False(string.IsNullOrWhiteSpace(programFiles));

        string unregisteredPath = Path.Combine(
            programFiles,
            $"DeskBoxWhite-Scope-Probe-{Guid.NewGuid():N}");

        Assert.Equal(
            DeskBoxWhite.Updater.DirectInstallScope.AllUsers,
            DeskBoxWhite.Updater.Program.ResolveInstallScope(unregisteredPath));
    }

    [Fact]
    public void PerUserInstallWithoutRegistration_DefaultsToCurrentUserScope()
    {
        string localAppData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);
        Assert.False(string.IsNullOrWhiteSpace(localAppData));

        string unregisteredPath = Path.Combine(
            localAppData,
            $"DeskBoxWhite-Scope-Probe-{Guid.NewGuid():N}");

        Assert.Equal(
            DeskBoxWhite.Updater.DirectInstallScope.CurrentUser,
            DeskBoxWhite.Updater.Program.ResolveInstallScope(unregisteredPath));
    }

    [Theory]
    [InlineData(2, "cancelled")]
    [InlineData(5, "cancelled")]
    [InlineData(20, "path-mismatch")]
    [InlineData(1, "failed")]
    [InlineData(99, "failed")]
    public void IncompleteInstallerExitCode_MapsToRecoveryOutcome(int exitCode, string expected)
    {
        Assert.Equal(expected, DeskBoxWhite.Updater.Program.GetIncompleteUpdateOutcome(exitCode));
    }
}
