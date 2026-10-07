using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DeskBoxWhite.Tests;

public sealed class AotPublishContractTests
{
    [Fact]
    public void AuditProfile_IsOptInAndPropagatesAotToUpdater()
    {
        XDocument project = XDocument.Load(TestPaths.FromRepository("src/DeskBoxWhite/DeskBoxWhite.csproj"));

        XElement defaultAuditProperty = project
            .Descendants("DeskBoxWhiteAotAudit")
            .Single(element => element.Attribute("Condition") is not null);
        Assert.Equal("false", defaultAuditProperty.Value);

        XElement auditGroup = project
            .Descendants("PropertyGroup")
            .Single(element => (string?)element.Attribute("Condition") == "'$(DeskBoxWhiteAotAudit)' == 'true'");
        Assert.Equal("true", auditGroup.Element("PublishAot")?.Value);
        Assert.Equal("true", auditGroup.Element("SelfContained")?.Value);
        Assert.Equal("false", auditGroup.Element("WindowsAppSDKSelfContained")?.Value);
        Assert.Equal("false", auditGroup.Element("PublishSingleFile")?.Value);

        XElement nativeAotGroup = project
            .Descendants("PropertyGroup")
            .Single(element => (string?)element.Attribute("Condition") == "'$(PublishAot)' == 'true'");
        Assert.Contains(
            "DESKBOXWHITE_NATIVE_AOT",
            nativeAotGroup.Element("DefineConstants")?.Value,
            StringComparison.Ordinal);

        XElement publishUpdater = project
            .Descendants("Target")
            .Single(element => (string?)element.Attribute("Name") == "PublishDeskBoxWhiteUpdater");
        string properties = Assert.IsType<string>((string?)publishUpdater.Element("MSBuild")?.Attribute("Properties"));
        Assert.Contains("DeskBoxWhiteAotAudit=$(DeskBoxWhiteAotAudit)", properties, StringComparison.Ordinal);
        Assert.Contains("PublishAot=$(PublishAot)", properties, StringComparison.Ordinal);
        Assert.Contains(
            "IlcUseEnvironmentalTools=$(IlcUseEnvironmentalTools)",
            properties,
            StringComparison.Ordinal);
    }

    [Fact]
    public void AuditProfile_DisablesDefaultJsonReflectionOnlyForAuditBuilds()
    {
        foreach (string projectPath in new[]
                 {
                     "src/DeskBoxWhite/DeskBoxWhite.csproj",
                     "src/DeskBoxWhite.Updater/DeskBoxWhite.Updater.csproj"
                 })
        {
            XDocument project = XDocument.Load(TestPaths.FromRepository(projectPath));
            XElement reflectionSwitch = Assert.Single(
                project.Descendants("JsonSerializerIsReflectionEnabledByDefault"));

            Assert.Equal("false", reflectionSwitch.Value);
            Assert.Equal(
                "'$(DeskBoxWhiteAotAudit)' == 'true'",
                (string?)reflectionSwitch.Parent?.Attribute("Condition"));
        }

        string script = File.ReadAllText(
            TestPaths.FromRepository("scripts/publish-aot-audit.ps1"));
        const string reflectionArgument =
            "\"-p:JsonSerializerIsReflectionEnabledByDefault=$($jsonSerializerIsReflectionEnabledByDefault.ToString().ToLowerInvariant())\"";

        Assert.Contains(
            "$jsonSerializerIsReflectionEnabledByDefault = $false",
            script,
            StringComparison.Ordinal);
        Assert.Equal(
            2,
            Regex.Matches(script, Regex.Escape(reflectionArgument)).Count);
        Assert.Contains("jsonSerializer = [ordered]@{", script, StringComparison.Ordinal);
        Assert.Contains(
            "reflectionEnabledByDefault = $jsonSerializerIsReflectionEnabledByDefault",
            script,
            StringComparison.Ordinal);
    }

    [Fact]
    public void AuditProfile_DoesNotCopyManagedUpdaterBuildOutput()
    {
        XDocument project = XDocument.Load(TestPaths.FromRepository("src/DeskBoxWhite/DeskBoxWhite.csproj"));

        foreach (string targetName in new[] { "BuildDeskBoxWhiteUpdater", "CopyDeskBoxWhiteUpdater" })
        {
            XElement target = project
                .Descendants("Target")
                .Single(element => (string?)element.Attribute("Name") == targetName);
            string condition = Assert.IsType<string>((string?)target.Attribute("Condition"));
            Assert.Contains("'$(PublishAot)' != 'true'", condition, StringComparison.Ordinal);
        }
    }

    [Fact]
    public async Task NativeAotConfigurationValidation_AllowsCompleteX64AndArm64Combinations()
    {
        ProcessResult ordinaryJit = await RunAotConfigurationValidationAsync(
            "Platform=x64",
            "RuntimeIdentifier=win-x64");
        Assert.Equal(0, ordinaryJit.ExitCode);

        ProcessResult directAot = await RunAotConfigurationValidationAsync(
            "PublishAot=true",
            "DeskBoxWhiteRustNative=true",
            "Platform=x64",
            "RuntimeIdentifier=win-x64");
        Assert.Equal(0, directAot.ExitCode);

        ProcessResult auditAot = await RunAotConfigurationValidationAsync(
            "DeskBoxWhiteAotAudit=true",
            "DeskBoxWhiteRustNative=true",
            "Platform=x64",
            "RuntimeIdentifier=win-x64");
        Assert.Equal(0, auditAot.ExitCode);

        ProcessResult arm64Aot = await RunAotConfigurationValidationAsync(
            "PublishAot=true",
            "DeskBoxWhiteRustNative=true",
            "Platform=ARM64",
            "RuntimeIdentifier=win-arm64");
        Assert.Equal(0, arm64Aot.ExitCode);
    }

    [Theory]
    [InlineData("PublishAot=true", "DeskBoxWhiteRustNative=false")]
    [InlineData("DeskBoxWhiteAotAudit=true", "DeskBoxWhiteRustNative=false")]
    public async Task NativeAotConfigurationValidation_RejectsMissingRustModule(
        string aotProperty,
        string rustProperty)
    {
        ProcessResult result = await RunAotConfigurationValidationAsync(
            aotProperty,
            rustProperty,
            "Platform=x64",
            "RuntimeIdentifier=win-x64");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains(
            "requires DeskBoxWhiteRustNative=true",
            result.Output,
            StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("x64", "win-arm64")]
    [InlineData("ARM64", "win-x64")]
    public async Task NativeAotConfigurationValidation_RejectsMismatchedArchitecture(
        string platform,
        string runtimeIdentifier)
    {
        ProcessResult result = await RunAotConfigurationValidationAsync(
            "PublishAot=true",
            "DeskBoxWhiteRustNative=true",
            $"Platform={platform}",
            $"RuntimeIdentifier={runtimeIdentifier}");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains(
            "requires a matching Platform/RuntimeIdentifier pair",
            result.Output,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task AotAudit_RejectsArm64BeforeResolvingToolsOrTouchingArtifacts()
    {
        string scriptPath = TestPaths.FromRepository("scripts/publish-aot-audit.ps1");
        string script = File.ReadAllText(scriptPath);
        int guard = script.IndexOf("if ($Platform -ne \"x64\")", StringComparison.Ordinal);
        int dotnetResolution = script.IndexOf("$dotnet = if", StringComparison.Ordinal);
        int artifactCleanup = script.IndexOf("Remove-Item -LiteralPath $runRoot", StringComparison.Ordinal);

        Assert.InRange(guard, 0, dotnetResolution - 1);
        Assert.InRange(guard, 0, artifactCleanup - 1);
        Assert.Contains("$rustNativeEnabled = $true", script, StringComparison.Ordinal);
        Assert.DoesNotContain("$rustNativeEnabled = $Platform -eq \"x64\"", script, StringComparison.Ordinal);

        ProcessResult result = await RunProcessAsync(
            "powershell.exe",
            "-NoProfile",
            "-ExecutionPolicy",
            "Bypass",
            "-File",
            scriptPath,
            "-Platform",
            "ARM64",
            "-DotNetPath",
            Path.Combine(Path.GetTempPath(), $"missing-dotnet-{Guid.NewGuid():N}.exe"));

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains(
            "currently supports only x64",
            result.Output,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "explicitly selected dotnet host does not exist",
            result.Output,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToolchainsAndAuditScript_ArePinnedAndValidateNativeOutput()
    {
        using JsonDocument globalJson = JsonDocument.Parse(
            File.ReadAllText(TestPaths.FromRepository("global.json")));
        JsonElement sdk = globalJson.RootElement.GetProperty("sdk");
        Assert.Equal("10.0.303", sdk.GetProperty("version").GetString());
        Assert.Equal("latestPatch", sdk.GetProperty("rollForward").GetString());

        string rustToolchain = File.ReadAllText(TestPaths.FromRepository("rust-toolchain.toml"));
        Assert.Contains("channel = \"1.96.0\"", rustToolchain, StringComparison.Ordinal);
        Assert.Contains("x86_64-pc-windows-msvc", rustToolchain, StringComparison.Ordinal);

        string script = File.ReadAllText(TestPaths.FromRepository("scripts/publish-aot-audit.ps1"));
        Assert.Contains("-p:DeskBoxWhiteAotAudit=true", script, StringComparison.Ordinal);
        Assert.Contains("coreclr.dll", script, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("DeskBoxWhite.Updater.dll", script, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Get-PeMachine", script, StringComparison.Ordinal);
        Assert.Contains("Move-Item", script, StringComparison.Ordinal);
        Assert.DoesNotContain(".codex-temp", script, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("gitDirty", script, StringComparison.Ordinal);
        Assert.Contains("workingTreeFingerprint", script, StringComparison.Ordinal);
        Assert.Contains("dotnetSdkVersion", script, StringComparison.Ordinal);
        Assert.Contains("--artifacts-path", script, StringComparison.Ordinal);
        Assert.Contains("buildArtifactsDirectory", script, StringComparison.Ordinal);
        Assert.Contains("DeskBoxWhite.pdb", script, StringComparison.Ordinal);
        Assert.Contains("DeskBoxWhite.Updater.pdb", script, StringComparison.Ordinal);
    }

    [Fact]
    public void RustNativeStage3C2_ImplementsReadWriteAndWindowsUiAbi2Capabilities()
    {
        string workspace = File.ReadAllText(TestPaths.FromRepository("native/Cargo.toml"));
        string crate = File.ReadAllText(TestPaths.FromRepository("native/deskboxwhite-native/Cargo.toml"));
        string source = File.ReadAllText(TestPaths.FromRepository("native/deskboxwhite-native/src/lib.rs"));
        string shortcutSource = File.ReadAllText(
            TestPaths.FromRepository("native/deskboxwhite-native/src/shortcut.rs"));
        string header = File.ReadAllText(TestPaths.FromRepository("native/include/deskboxwhite_native.h"));
        string buildScript = File.ReadAllText(TestPaths.FromRepository("scripts/build-rust-native.ps1"));
        string contract = File.ReadAllText(
            TestPaths.FromRepository("docs/architecture/shortcut-native-abi-v2.md"));
        string lockFile = File.ReadAllText(TestPaths.FromRepository("native/Cargo.lock"));

        Assert.Contains("members = [", workspace, StringComparison.Ordinal);
        Assert.Contains("\"deskboxwhite-native\"", workspace, StringComparison.Ordinal);
        Assert.Contains("\"deskboxwhite-audio-session-fixture\"", workspace, StringComparison.Ordinal);
        Assert.DoesNotContain("\"deskboxwhite-search-core\"", workspace, StringComparison.Ordinal);
        Assert.Contains("panic = \"abort\"", workspace, StringComparison.Ordinal);
        Assert.Contains("crate-type = [\"cdylib\"]", crate, StringComparison.Ordinal);
        Assert.Contains("rust-version = \"1.96\"", crate, StringComparison.Ordinal);
        Assert.Contains("windows = { version = \"0.62.2\"", crate, StringComparison.Ordinal);
        Assert.Contains("\"Win32_System_Com\"", crate, StringComparison.Ordinal);
        Assert.Contains("\"Win32_UI_Shell\"", crate, StringComparison.Ordinal);
        Assert.Contains("pub const DESKBOXWHITE_NATIVE_ABI_VERSION: u32 = 2", source, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_READ_STORED_RAW_V2", source, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_READ_EFFECTIVE_DIAGNOSTIC_V2", source, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_WRITE_V2", source, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_RESOLVE_WITH_UI_V2", source, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_NATIVE_CAPABILITY_MUSIC_VOLUME_V1", source, StringComparison.Ordinal);
        Assert.DoesNotContain("pub const DESKBOXWHITE_NATIVE_CAPABILITIES: u64 = 0", source, StringComparison.Ordinal);
        Assert.Contains("pub extern \"C\" fn deskboxwhite_native_abi_version()", source, StringComparison.Ordinal);
        Assert.Contains("pub extern \"C\" fn deskboxwhite_native_capabilities()", source, StringComparison.Ordinal);
        Assert.Contains("pub unsafe extern \"C\" fn deskboxwhite_shortcut_read_v2(", source, StringComparison.Ordinal);
        Assert.Contains("pub unsafe extern \"C\" fn deskboxwhite_shortcut_resolve_no_ui_v2(", source, StringComparison.Ordinal);
        Assert.Contains("pub unsafe extern \"C\" fn deskboxwhite_shortcut_write_v2(", source, StringComparison.Ordinal);
        Assert.Contains("pub unsafe extern \"C\" fn deskboxwhite_shortcut_resolve_with_ui_v2(", source, StringComparison.Ordinal);
        Assert.Contains("shortcut::read_shortcut", source, StringComparison.Ordinal);
        Assert.Contains("shortcut::resolve_shortcut", source, StringComparison.Ordinal);
        Assert.Contains("shortcut::write_shortcut", source, StringComparison.Ordinal);
        Assert.Contains("shortcut::resolve_shortcut_with_ui", source, StringComparison.Ordinal);
        Assert.Contains("CoInitializeEx", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("RPC_E_CHANGED_MODE", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("SLGP_RAWPATH", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("SLR_NO_UI", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("SLR_NOSEARCH", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("SLR_UPDATE", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("SLR_OFFER_DELETE_WITHOUT_FILE", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_SHORTCUT_PHASE_RESOLVE", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_SHORTCUT_PHASE_SAVE", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("SetPath", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("SetDescription", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("SetArguments", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("SetWorkingDirectory", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("SetIconLocation", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("DIAGNOSTIC_ARGUMENT_CAPACITY: usize = 512", shortcutSource, StringComparison.Ordinal);
        Assert.Contains("#define DESKBOXWHITE_NATIVE_ABI_VERSION 2u", header, StringComparison.Ordinal);
        Assert.Contains("#define DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_3C2", header, StringComparison.Ordinal);
        Assert.Contains("#define DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_4C", header, StringComparison.Ordinal);
        Assert.Contains("#define DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_4D4A", header, StringComparison.Ordinal);
        Assert.Contains("#define DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_4D4B", header, StringComparison.Ordinal);
        Assert.Contains("#define DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_5B4C1B1", header, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_NATIVE_CAPABILITY_RECYCLE_BIN_V1", header, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_NATIVE_CAPABILITIES DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_5B4C1B1", header, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_SHORTCUT_READ_REQUEST_V2_SIZE_64 144u", header, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_SHORTCUT_READ_RESULT_V2_SIZE_64 136u", header, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_SHORTCUT_RESOLVE_REQUEST_V2_SIZE_64 192u", header, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_SHORTCUT_WRITE_REQUEST_V2_SIZE_64 144u", header, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_SHORTCUT_WRITE_RESULT_V2_SIZE_64 96u", header, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_SHORTCUT_UI_RESOLVE_REQUEST_V2_SIZE_64 64u", header, StringComparison.Ordinal);
        Assert.Contains("DESKBOXWHITE_SHORTCUT_UI_RESOLVE_RESULT_V2_SIZE_64 64u", header, StringComparison.Ordinal);
        Assert.Contains("typedef struct DeskBoxWhiteShortcutReadRequestV2", header, StringComparison.Ordinal);
        Assert.Contains("typedef struct DeskBoxWhiteShortcutReadResultV2", header, StringComparison.Ordinal);
        Assert.Contains("typedef struct DeskBoxWhiteShortcutResolveRequestV2", header, StringComparison.Ordinal);
        Assert.Contains("typedef struct DeskBoxWhiteShortcutWriteRequestV2", header, StringComparison.Ordinal);
        Assert.Contains("typedef struct DeskBoxWhiteShortcutWriteResultV2", header, StringComparison.Ordinal);
        Assert.Contains("typedef struct DeskBoxWhiteShortcutUiResolveRequestV2", header, StringComparison.Ordinal);
        Assert.Contains("typedef struct DeskBoxWhiteShortcutUiResolveResultV2", header, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_native_abi_version(void)", header, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_native_capabilities(void)", header, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_shortcut_read_v2(", header, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_shortcut_resolve_no_ui_v2(", header, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_shortcut_write_v2(", header, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_shortcut_resolve_with_ui_v2(", header, StringComparison.Ordinal);
        Assert.Contains("--locked", buildScript, StringComparison.Ordinal);
        Assert.Contains("x86_64-pc-windows-msvc", buildScript, StringComparison.Ordinal);
        Assert.Contains("ReadContract", buildScript, StringComparison.Ordinal);
        Assert.Contains("Rust native ABI mismatch", buildScript, StringComparison.Ordinal);
        Assert.Contains("Rust native Stage 5B-4C1B2B capability mismatch: expected 511", buildScript, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_shortcut_read_v2", buildScript, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_shortcut_resolve_no_ui_v2", buildScript, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_shortcut_write_v2", buildScript, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_shortcut_resolve_with_ui_v2", buildScript, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_explorer_shell_launch_v1", buildScript, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_quick_access_v1", buildScript, StringComparison.Ordinal);
        Assert.Contains("CargoTargetDirectory", buildScript, StringComparison.Ordinal);
        Assert.Contains("--target-dir", buildScript, StringComparison.Ordinal);
        Assert.Contains("ValidateOnly", buildScript, StringComparison.Ordinal);
        Assert.Contains("能力掩码", contract, StringComparison.Ordinal);
        Assert.Contains("RPC_E_CHANGED_MODE", contract, StringComparison.Ordinal);
        Assert.Contains("SLR_NO_UI | SLR_NOSEARCH", contract, StringComparison.Ordinal);
        Assert.Contains(
            "SLR_UPDATE | SLR_NOSEARCH | SLR_OFFER_DELETE_WITHOUT_FILE",
            contract,
            StringComparison.Ordinal);
        Assert.Contains("name = \"windows\"", lockFile, StringComparison.Ordinal);
        Assert.Contains("version = \"0.62.2\"", lockFile, StringComparison.Ordinal);
    }

    [Fact]
    public void RustNativeStage3C2_HasSafeExplicitProductLoaderWithoutFallback()
    {
        string loader = File.ReadAllText(
            TestPaths.FromRepository("src/DeskBoxWhite/Helpers/ShortcutNativeBackend.cs"));
        string helper = File.ReadAllText(
            TestPaths.FromRepository("src/DeskBoxWhite/Helpers/ShortcutHelper.cs"));
        string dragDrop = File.ReadAllText(
            TestPaths.FromRepository("src/DeskBoxWhite/Services/DragDropPermissionService.cs"));
        string testProject = File.ReadAllText(
            TestPaths.FromRepository("tests/DeskBoxWhite.Tests/DeskBoxWhite.Tests.csproj"));
        string differentialTests = File.ReadAllText(
            TestPaths.FromRepository("tests/DeskBoxWhite.Tests/ShortcutNativeDifferentialTests.cs"));
        string fileNavigation = File.ReadAllText(
            TestPaths.FromRepository(
                "src/DeskBoxWhite/Controls/WidgetContents/FileSurfaceContent.Navigation.cs"));

        Assert.Contains("DESKBOXWHITE_SHORTCUT_BACKEND", loader, StringComparison.Ordinal);
        Assert.Contains("RuntimeFeature.IsDynamicCodeSupported", loader, StringComparison.Ordinal);
        Assert.Contains("return ShortcutBackendMode.Rust", loader, StringComparison.Ordinal);
        Assert.Contains("Kernel32NativeMethods.LoadLibraryEx(", loader, StringComparison.Ordinal);
        Assert.Contains("LoadLibrarySearchDllLoadDir", loader, StringComparison.Ordinal);
        Assert.Contains("LoadLibrarySearchSystem32", loader, StringComparison.Ordinal);
        Assert.Contains("Path.Combine(AppContext.BaseDirectory, DllName)", loader, StringComparison.Ordinal);
        Assert.Contains("NativeLibrary.TryGetExport", loader, StringComparison.Ordinal);
        Assert.Contains("delegate* unmanaged[Cdecl]", loader, StringComparison.Ordinal);
        Assert.Contains("CapabilityUnavailable", loader, StringComparison.Ordinal);
        Assert.Contains("WriteCapability", loader, StringComparison.Ordinal);
        Assert.Contains("ShortcutNativeWriteCallResult", loader, StringComparison.Ordinal);
        Assert.Contains("ResolveWithUiCapability", loader, StringComparison.Ordinal);
        Assert.Contains("ShortcutNativeUiResolveCallResult", loader, StringComparison.Ordinal);
        Assert.DoesNotContain("DllImport(\"deskboxwhite_native", loader, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("LibraryImport(\"deskboxwhite_native", loader, StringComparison.OrdinalIgnoreCase);

        int urlDispatch = helper.IndexOf(".Equals(\".url\"", StringComparison.Ordinal);
        int backendDispatch = helper.IndexOf("ShortcutBackendPolicy.Current", StringComparison.Ordinal);
        Assert.True(urlDispatch >= 0 && backendDispatch > urlDispatch);
        Assert.Contains("ShortcutNativeBackend.ReadStoredRaw", helper, StringComparison.Ordinal);
        Assert.Contains("ShortcutNativeBackend.ResolveNoUi", helper, StringComparison.Ordinal);
        Assert.Contains("ShortcutNativeBackend.WriteShortcut", helper, StringComparison.Ordinal);
        Assert.Contains("ShortcutNativeBackend.WriteShellNamespaceShortcut", helper, StringComparison.Ordinal);
        Assert.Contains("CreateShellApplicationShortcut", helper, StringComparison.Ordinal);
        Assert.Contains("ShortcutNativeBackend.ResolveWithUi", helper, StringComparison.Ordinal);
        Assert.Contains("Explicit Rust", helper, StringComparison.Ordinal);
        Assert.Contains("ShortcutNativeBackend.ReadEffectiveDiagnostic", dragDrop, StringComparison.Ordinal);
        Assert.Contains("ShortcutNativeBackend.WriteShortcut", dragDrop, StringComparison.Ordinal);
        Assert.Contains("return native.Success", dragDrop, StringComparison.Ordinal);

        Assert.Contains("BuildDeskBoxWhiteNativeTestModule", testProject, StringComparison.Ordinal);
        Assert.Contains("ShortcutNativeDifferentialTests", differentialTests, StringComparison.Ordinal);
        Assert.Contains("StoredRaw_ConcurrentReadsMatchCSharpOracle", differentialTests, StringComparison.Ordinal);
        Assert.Contains("PidlOnlyShortcut", differentialTests, StringComparison.Ordinal);
        Assert.Contains("Write_AllFieldsAndNegativeIconIndexMatchCSharpOracle", differentialTests, StringComparison.Ordinal);
        Assert.Contains("ApplicationShortcutWriteInvalidatesStoredMetadataCache", differentialTests, StringComparison.Ordinal);
        Assert.Contains("FolderShortcutWriteCreatesParentAndInvalidatesStoredMetadataCache", differentialTests, StringComparison.Ordinal);
        Assert.Contains("ResolveWithUi_ValidShortcutForwardsOwnerAndFrozenFlags", differentialTests, StringComparison.Ordinal);
        Assert.Contains("ApplicationShortcutUiResolveKeepsLinkAndInvalidatesStoredMetadataCache", differentialTests, StringComparison.Ordinal);
        Assert.Contains("await OpenFileItemAsync(item)", fileNavigation, StringComparison.Ordinal);
    }

    [Fact]
    public void RustNativeStage3C3_ClosesAotComAndOwnerlessUiPathsWithoutDiagnosticLoads()
    {
        string loader = File.ReadAllText(
            TestPaths.FromRepository("src/DeskBoxWhite/Helpers/ShortcutNativeBackend.cs"));
        string helper = File.ReadAllText(
            TestPaths.FromRepository("src/DeskBoxWhite/Helpers/ShortcutHelper.cs"));
        string dragDrop = File.ReadAllText(
            TestPaths.FromRepository("src/DeskBoxWhite/Services/DragDropPermissionService.cs"));
        string operations = File.ReadAllText(
            TestPaths.FromRepository("src/DeskBoxWhite/ViewModels/WidgetViewModel.Operations.cs"));
        string fileService = File.ReadAllText(
            TestPaths.FromRepository("src/DeskBoxWhite/Services/FileService.cs"));
        string diagnostics = File.ReadAllText(
            TestPaths.FromRepository("src/DeskBoxWhite/Services/DeskBoxWhiteDiagnosticsBundleService.cs"));

        Assert.Contains("#if DESKBOXWHITE_NATIVE_AOT", loader, StringComparison.Ordinal);
        Assert.Contains("#if !DESKBOXWHITE_NATIVE_AOT", helper, StringComparison.Ordinal);
        Assert.Contains("#if !DESKBOXWHITE_NATIVE_AOT", dragDrop, StringComparison.Ordinal);
        Assert.DoesNotContain("public void OpenItem(WidgetItem item)", operations, StringComparison.Ordinal);
        Assert.Contains(
            "public FileService.OpenItemResult OpenItem(WidgetItem item, IntPtr ownerHwnd)",
            operations,
            StringComparison.Ordinal);
        Assert.Contains(
            "public static OpenItemResult OpenItem(WidgetItem item, IntPtr ownerHwnd)",
            fileService,
            StringComparison.Ordinal);
        Assert.Contains(
            "public static async Task<OpenItemResult> OpenItemAsync(",
            File.ReadAllText(TestPaths.FromRepository(
                "src/DeskBoxWhite/Services/FileService.OpenItem.cs")),
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "OpenItem(WidgetItem item, IntPtr ownerHwnd = default)",
            fileService,
            StringComparison.Ordinal);

        int captureStart = loader.IndexOf(
            "internal static ShortcutNativeDiagnosticState CaptureDiagnosticState()",
            StringComparison.Ordinal);
        int captureEnd = loader.IndexOf(
            "internal static ShortcutNativeCallResult ReadStoredRaw",
            captureStart,
            StringComparison.Ordinal);
        Assert.True(captureStart >= 0 && captureEnd > captureStart);
        string diagnosticCapture = loader[captureStart..captureEnd];
        Assert.Contains("TryGetCachedDefault", diagnosticCapture, StringComparison.Ordinal);
        Assert.Contains("SHA256.HashData", loader, StringComparison.Ordinal);
        Assert.DoesNotContain("ShortcutNativeModule.Default", diagnosticCapture, StringComparison.Ordinal);
        Assert.DoesNotContain(".Detail", diagnosticCapture, StringComparison.Ordinal);
        Assert.Contains("DeskBoxWhiteShortcutNativeDiagnostic", diagnostics, StringComparison.Ordinal);
    }

    [Fact]
    public void RustNativeStage7A_RemainsOptInAndCopiesTheSelectedArchitectureModule()
    {
        XDocument project = XDocument.Load(TestPaths.FromRepository("src/DeskBoxWhite/DeskBoxWhite.csproj"));

        XElement defaultRustProperty = project
            .Descendants("DeskBoxWhiteRustNative")
            .Single(element => element.Attribute("Condition") is not null);
        Assert.Equal("false", defaultRustProperty.Value);

        XElement buildTarget = project
            .Descendants("Target")
            .Single(element => (string?)element.Attribute("Name") == "BuildDeskBoxWhiteRustNative");
        string buildCondition = Assert.IsType<string>((string?)buildTarget.Attribute("Condition"));
        string platformErrors = string.Join(
            Environment.NewLine,
            buildTarget.Elements("Error").Select(element => (string?)element.Attribute("Text")));
        Assert.Contains("'$(DeskBoxWhiteRustNative)' == 'true'", buildCondition, StringComparison.Ordinal);
        Assert.Contains("RuntimeIdentifier=win-x64 or win-arm64", platformErrors, StringComparison.Ordinal);
        Assert.Contains("Platform=x64, ARM64", platformErrors, StringComparison.Ordinal);
        Assert.Contains("matching Platform and RuntimeIdentifier", platformErrors, StringComparison.Ordinal);

        XElement noRidSegment = project
            .Descendants("DeskBoxWhiteRustNativeRuntimeSegment")
            .Single(element => (string?)element.Attribute("Condition") == "'$(RuntimeIdentifier)' == ''");
        XElement intermediateDirectory = project.Descendants("DeskBoxWhiteRustNativeIntermediateDir").Single();
        XElement cargoTargetDirectory = project.Descendants("DeskBoxWhiteRustNativeCargoTargetDir").Single();
        Assert.Equal("no-rid", noRidSegment.Value);
        Assert.Contains("$(DeskBoxWhiteRustNativeRuntimeSegment)", intermediateDirectory.Value, StringComparison.Ordinal);
        Assert.False(intermediateDirectory.Value.EndsWith('\\'));
        Assert.Contains("$(DeskBoxWhiteRustNativeIntermediateDir)", cargoTargetDirectory.Value, StringComparison.Ordinal);
        Assert.Contains(
            "-CargoTargetDirectory \"$(DeskBoxWhiteRustNativeCargoTargetDir)\"",
            (string?)buildTarget.Element("Exec")?.Attribute("Command"),
            StringComparison.Ordinal);

        Assert.Contains(
            project.Descendants("Target"),
            element => (string?)element.Attribute("Name") == "CopyDeskBoxWhiteRustNativeToOutput");
        Assert.Contains(
            project.Descendants("Target"),
            element => (string?)element.Attribute("Name") == "CopyDeskBoxWhiteRustNativeToPublish");
    }

    [Fact]
    public void AotAudit_RequiresAndFingerprintsTheX64RustModule()
    {
        string script = File.ReadAllText(TestPaths.FromRepository("scripts/publish-aot-audit.ps1"));

        Assert.Contains("DeskBoxWhiteRustNative=$($rustNativeEnabled", script, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_native.dll", script, StringComparison.Ordinal);
        Assert.Contains("deskboxwhite_native.pdb", script, StringComparison.Ordinal);
        Assert.Contains("-ValidateOnly", script, StringComparison.Ordinal);
        Assert.Contains("abiVersion = $rustAbiVersion", script, StringComparison.Ordinal);
        Assert.Contains("capabilities = $rustCapabilities", script, StringComparison.Ordinal);
        Assert.Contains("requiredExports = @($rustRequiredExports)", script, StringComparison.Ordinal);
        Assert.Contains("publishedNativeModules", script, StringComparison.Ordinal);
        Assert.Contains("publishMatchesStaging", script, StringComparison.Ordinal);
        Assert.Contains("exactly one root-level deskboxwhite_native.dll", script, StringComparison.Ordinal);
        Assert.DoesNotContain("deskboxwhite_search_core.dll", script, StringComparison.Ordinal);
        Assert.Contains("schemaVersion = 55", script, StringComparison.Ordinal);
        Assert.Contains("auditProfileVersion = 59", script, StringComparison.Ordinal);
        Assert.Contains("warningCodeCounts", script, StringComparison.Ordinal);
        Assert.Contains("targetedWarningCounts", script, StringComparison.Ordinal);
        Assert.Contains("workingTreeFingerprintBefore", script, StringComparison.Ordinal);
        Assert.Contains("workingTreeFingerprintAfter", script, StringComparison.Ordinal);
        Assert.Contains("sourceStableDuringAudit", script, StringComparison.Ordinal);
        Assert.Contains("core.quotepath=false", script, StringComparison.Ordinal);
        Assert.Contains("DeskBoxWhiteRustNativeCargoTargetDir", script, StringComparison.Ordinal);
        Assert.Contains("Get-PeImports", script, StringComparison.Ordinal);
        Assert.Contains("imports = @($imports)", script, StringComparison.Ordinal);
        Assert.Contains("cargo metadata", script, StringComparison.Ordinal);
        Assert.Contains("lockedPackageCount", script, StringComparison.Ordinal);
        Assert.Contains("lockedPackages = @($rustLockedPackages)", script, StringComparison.Ordinal);
        Assert.Contains("explicitOptInEnvironmentVariable = \"DESKBOXWHITE_SHORTCUT_BACKEND\"", script, StringComparison.Ordinal);
        Assert.Contains("fallbackOnNativeFailure = $false", script, StringComparison.Ordinal);
        Assert.Contains("nativeAotCompileTimeDefine = \"DESKBOXWHITE_NATIVE_AOT\"", script, StringComparison.Ordinal);
        Assert.Contains("allowedWarningCodes", script, StringComparison.Ordinal);
        Assert.Contains("shortcutAlwaysThrowMessages", script, StringComparison.Ordinal);
        Assert.DoesNotContain("FolderPickerService+FileOpenDialog", script, StringComparison.Ordinal);
        Assert.Contains("musicVolumeAlwaysThrowMessages", script, StringComparison.Ordinal);
        Assert.DoesNotContain("MusicVolumeService+MMDeviceEnumeratorComObject", script, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("src/DeskBoxWhite/ViewModels/SearchPopupViewModel.cs", 15)]
    [InlineData("src/DeskBoxWhite/ViewModels/SettingsViewModel.cs", 15)]
    public void AotSensitiveViewModels_UseObservablePartialProperties(
        string relativePath,
        int expectedCount)
    {
        string source = File.ReadAllText(TestPaths.FromRepository(relativePath));

        Assert.False(
            Regex.IsMatch(source, @"\[ObservableProperty\]\s+private\s+"),
            $"{relativePath} still contains field-based ObservableProperty declarations.");
        Assert.Equal(
            expectedCount,
            Regex.Matches(source, @"\[ObservableProperty\]\s+public\s+partial\s+").Count);
    }

    [Fact]
    public void TodoSettingsEditor_PreservesWritableAotBindingSurface()
    {
        // Batch 47 moved the Todo section's binding surface onto the
        // section editor; the writable AOT bridge follows it there.
        string bridge = File.ReadAllText(TestPaths.FromRepository(
            "src/DeskBoxWhite/Features/Todo/TodoSettingsViewModel.AotBindableProperties.cs"));
        foreach ((string name, Type expectedType) in new[]
        {
            ("Enabled", typeof(bool)),
            ("RemindersEnabled", typeof(bool)),
            ("DefaultOffsetMinutes", typeof(int))
        })
        {
            var property = typeof(DeskBoxWhite.Features.Todo.TodoSettingsViewModel).GetProperty(name);
            Assert.NotNull(property);
            Assert.Equal(expectedType, property!.PropertyType);
            Assert.True(property.CanRead && property.CanWrite);
            Assert.Contains($"nameof({name})", bridge, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void WinRtAbiTypes_ArePartial()
    {
        var expectedDeclarations = new Dictionary<string, string[]>
        {
            ["src/DeskBoxWhite/Controls/MarkdownDocumentView.cs"] =
                ["public sealed partial class MarkdownDocumentView"],
            ["src/DeskBoxWhite/Controls/WidgetItemTemplateSelector.cs"] =
                ["public sealed partial class WidgetItemTemplateSelector"],
            ["src/DeskBoxWhite/Controls/WidgetContents/GlanceWidgetContent.xaml.cs"] =
            [
                "public sealed partial class GlanceBoolToVisibilityConverter",
                "public sealed partial class GlanceInverseBoolToVisibilityConverter",
                "public sealed partial class GlanceBoolToFontWeightConverter"
            ],
            ["src/DeskBoxWhite/Controls/WidgetContents/WeatherWidgetContent.xaml.cs"] =
            [
                "internal sealed partial class TempBarMarginConverter",
                "internal sealed partial class BoolToVisibilityConverter"
            ]
        };

        foreach ((string relativePath, string[] declarations) in expectedDeclarations)
        {
            string source = File.ReadAllText(TestPaths.FromRepository(relativePath));
            foreach (string declaration in declarations)
            {
                Assert.Contains(declaration, source, StringComparison.Ordinal);
            }
        }
    }

    [Fact]
    public void SettingsViewModel_ConstructorSuppressesObservablePropertyCallbacks()
    {
        string source = File.ReadAllText(
            TestPaths.FromRepository("src/DeskBoxWhite/ViewModels/SettingsViewModel.cs"));
        int constructorStart = source.IndexOf("public SettingsViewModel(", StringComparison.Ordinal);
        Assert.True(constructorStart >= 0);

        int suppressionStart = source.IndexOf("_isRestoringDefaults = true;", constructorStart, StringComparison.Ordinal);
        Assert.True(suppressionStart >= 0);

        int firstMigratedAssignment = source.IndexOf("AutoStart = StartupService.IsEnabled();", constructorStart, StringComparison.Ordinal);
        // Batch 50 moved the performance trim toggles to the performance
        // editor; the hover-button selection remains the last facade
        // assignment covered by the constructor's suppression window.
        int lastMigratedAssignment = source.IndexOf(
            "ShowHoverButtons = settings.ShowHoverButtons;",
            constructorStart,
            StringComparison.Ordinal);
        int suppressionEnd = source.IndexOf("_isRestoringDefaults = false;", suppressionStart, StringComparison.Ordinal);
        int nextMember = source.IndexOf("[RelayCommand]", constructorStart, StringComparison.Ordinal);

        Assert.True(firstMigratedAssignment >= 0);
        Assert.True(lastMigratedAssignment >= 0);
        Assert.True(nextMember >= 0);
        Assert.InRange(suppressionStart, constructorStart, firstMigratedAssignment - 1);
        Assert.InRange(suppressionEnd, lastMigratedAssignment + 1, nextMember - 1);
    }

    private static Task<ProcessResult> RunAotConfigurationValidationAsync(params string[] properties)
    {
        var arguments = new List<string>
        {
            "msbuild",
            TestPaths.FromRepository("src/DeskBoxWhite/DeskBoxWhite.csproj"),
            "-nologo",
            "-t:ValidateDeskBoxWhiteNativeAotConfiguration",
            "-p:Configuration=Release"
        };
        arguments.AddRange(properties.Select(property => $"-p:{property}"));

        string dotnet = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";
        return RunProcessAsync(dotnet, arguments.ToArray());
    }

    private static async Task<ProcessResult> RunProcessAsync(
        string fileName,
        params string[] arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process { StartInfo = startInfo };
        Assert.True(process.Start(), $"Failed to start '{fileName}'.");
        Task<string> standardOutput = process.StandardOutput.ReadToEndAsync();
        Task<string> standardError = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        string output = string.Concat(
            await standardOutput,
            Environment.NewLine,
            await standardError);
        return new ProcessResult(process.ExitCode, output);
    }

    private sealed record ProcessResult(int ExitCode, string Output);
}
