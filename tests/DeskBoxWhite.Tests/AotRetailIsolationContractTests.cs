using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DeskBoxWhite.Tests;

public sealed class AotRetailIsolationContractTests
{
    private const string HarnessGuard =
        "#if DESKBOXWHITE_NATIVE_AOT && DESKBOXWHITE_AOT_SMOKE_HARNESS";

    [Fact]
    public void RetailCompile_DefaultsSmokeHarnessOffAndExcludesOnlyHarnessSources()
    {
        XDocument project = XDocument.Load(
            TestPaths.FromRepository("src/DeskBoxWhite/DeskBoxWhite.csproj"));

        XElement defaultProperty = Assert.Single(
            project.Descendants("DeskBoxWhiteAotSmokeHarness")
                .Where(element => (string?)element.Attribute("Condition") ==
                    "'$(DeskBoxWhiteAotSmokeHarness)' == ''"));
        Assert.Equal("false", defaultProperty.Value);

        XElement harnessConstants = Assert.Single(
            project.Descendants("PropertyGroup")
                .Where(element => (string?)element.Attribute("Condition") ==
                    "'$(PublishAot)' == 'true' and '$(DeskBoxWhiteAotSmokeHarness)' == 'true'"));
        Assert.Contains(
            "DESKBOXWHITE_AOT_SMOKE_HARNESS",
            harnessConstants.Element("DefineConstants")?.Value,
            StringComparison.Ordinal);

        XElement retailExclusions = Assert.Single(
            project.Descendants("ItemGroup")
                .Where(element => (string?)element.Attribute("Condition") ==
                    "'$(DeskBoxWhiteAotSmokeHarness)' != 'true'"));
        string[] removedPatterns = retailExclusions.Elements("Compile")
            .Select(element => (string?)element.Attribute("Remove"))
            .OfType<string>()
            .ToArray();
        Assert.Equal(
            ["**\\*.Aot*Smoke.cs", "Services\\Aot*Fixture.cs"],
            removedPatterns);

        string sourceRoot = TestPaths.FromRepository("src/DeskBoxWhite");
        string[] harnessSources = Directory
            .EnumerateFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
            .Where(IsSmokeHarnessSource)
            .ToArray();
        Assert.Equal(61, harnessSources.Length);
        Assert.DoesNotContain(
            harnessSources,
            path => path.EndsWith(".AotBindableProperties.cs", StringComparison.Ordinal));
    }

    [Fact]
    public void ProductHooks_RequireBothNativeAotAndExplicitSmokeHarness()
    {
        foreach (string relativePath in new[]
                 {
                     "src/DeskBoxWhite/App.xaml.cs",
                     "src/DeskBoxWhite/Helpers/NativeDropTarget.cs",
                     "src/DeskBoxWhite/Helpers/ShellContextMenuHelper.cs",
                     "src/DeskBoxWhite/Services/FileService.cs",
                     "src/DeskBoxWhite/Services/LocalizationService.cs",
                     "src/DeskBoxWhite/Services/OrganizerService.cs",
                     "src/DeskBoxWhite/Services/WeatherWidgetContentProvider.cs"
                 })
        {
            string source = File.ReadAllText(TestPaths.FromRepository(relativePath));
            Assert.Contains(HarnessGuard, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void AuditStoreAndDirectDistribution_UseExplicitOppositeHarnessProfiles()
    {
        string x64Audit = Read("scripts/publish-aot-audit.ps1");
        string arm64Audit = Read("scripts/publish-arm64-aot-static-audit.ps1");
        string storeBuild = Read("scripts/build-store-msix.ps1");
        string retailPublish = Read("scripts/publish-aot-retail.ps1");
        string distribution = Read("scripts/build-stage-7c1-distribution.ps1");

        Assert.Equal(
            2,
            Regex.Matches(x64Audit, "-p:DeskBoxWhiteAotSmokeHarness=true").Count);
        Assert.Contains("-p:DeskBoxWhiteAotSmokeHarness=true", arm64Audit, StringComparison.Ordinal);
        Assert.Contains("smokeHarnessEnabled = $true", x64Audit, StringComparison.Ordinal);
        Assert.Contains("smokeHarnessEnabled = $true", arm64Audit, StringComparison.Ordinal);

        Assert.Contains("-p:DeskBoxWhiteAotSmokeHarness=false", storeBuild, StringComparison.Ordinal);
        Assert.Contains("-p:DeskBoxWhiteAotSmokeHarness=false", retailPublish, StringComparison.Ordinal);
        Assert.Contains("-p:SelfContained=true", retailPublish, StringComparison.Ordinal);
        Assert.Contains("-p:WindowsAppSDKSelfContained=true", retailPublish, StringComparison.Ordinal);
        Assert.Contains("productProfile = \"retail\"", retailPublish, StringComparison.Ordinal);
        Assert.Contains("deploymentProfile = \"full\"", retailPublish, StringComparison.Ordinal);
        Assert.Contains("installManifestFileCount", retailPublish, StringComparison.Ordinal);
        Assert.Contains("smokeHarnessEnabled = $false", retailPublish, StringComparison.Ordinal);
        Assert.Contains("smokeHarnessBinaryMatches", retailPublish, StringComparison.Ordinal);
        Assert.Contains(
            "$ascii.IndexOf($_, [System.StringComparison]::Ordinal) -ge 0",
            retailPublish,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "$ascii.Contains($_, [System.StringComparison]::Ordinal)",
            retailPublish,
            StringComparison.Ordinal);
        Assert.Contains(
            "$retailPublishArguments = @{",
            distribution,
            StringComparison.Ordinal);
        Assert.Contains(
            "Platform = $Platform",
            distribution,
            StringComparison.Ordinal);
        Assert.Contains(
            "\"/DDeskBoxWhiteBundledRuntime=1\"",
            distribution,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "$retailPublishArguments = @(\"-Platform\", $Platform)",
            distribution,
            StringComparison.Ordinal);

        int retainedAudit = distribution.IndexOf(
            "& $directAuditScript @directAuditArguments",
            StringComparison.Ordinal);
        int retail = distribution.IndexOf(
            "& $retailPublishScript @retailPublishArguments",
            StringComparison.Ordinal);
        Assert.InRange(retainedAudit, 0, retail - 1);
        Assert.Contains("direct-aot-retail-summary.json", distribution, StringComparison.Ordinal);
        Assert.Contains("smokeHarnessEnabled = $false", distribution, StringComparison.Ordinal);
    }

    private static bool IsSmokeHarnessSource(string path)
    {
        string fileName = Path.GetFileName(path);
        bool smokePartial =
            fileName.Contains(".Aot", StringComparison.Ordinal) &&
            fileName.EndsWith("Smoke.cs", StringComparison.Ordinal);
        bool fixture =
            fileName.StartsWith("Aot", StringComparison.Ordinal) &&
            fileName.EndsWith("Fixture.cs", StringComparison.Ordinal) &&
            string.Equals(
                Path.GetFileName(Path.GetDirectoryName(path)),
                "Services",
                StringComparison.Ordinal);
        return smokePartial || fixture;
    }

    private static string Read(string relativePath) =>
        File.ReadAllText(TestPaths.FromRepository(relativePath));
}
