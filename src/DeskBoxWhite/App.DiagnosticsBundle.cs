using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using DeskBoxWhite.Helpers;
using DeskBoxWhite.Models;
using DeskBoxWhite.Platform;
using DeskBoxWhite.Services;

namespace DeskBoxWhite;

public partial class App
{
    public DeskBoxWhiteDiagnosticsBundleService DiagnosticsBundleService { get; private set; } = null!;

    public DeskBoxWhiteDiagnosticSnapshot CreateDiagnosticSnapshot()
    {
        AppRuntimeHealthSnapshot? runtimeHealth = DiagnosticsService?.GetRuntimeHealthSnapshot(
            EverythingSearchService,
            WidgetManager?.GetFolderWatcherHealthSnapshots());
        DeskBoxWhiteWidgetManagerDiagnostic widgetManager =
            WidgetManager?.CreateDiagnosticsSnapshot() ?? DeskBoxWhiteWidgetManagerDiagnostic.Empty;
        IReadOnlyList<DeskBoxWhiteDisplayDiagnostic> displays = GetDisplayDiagnostics();
        SettingsPersistenceFailure? saveFailure = SettingsService.LastPersistenceFailure;
        GlobalHotkeyGesture toggleGesture = GlobalHotkeyService?.CurrentGesture ??
            GlobalHotkeyService.NormalizeGesture(
                SettingsService.Settings.GlobalHotkeyModifiers,
                SettingsService.Settings.GlobalHotkeyKey);
        GlobalHotkeyGesture searchGesture = SearchHotkeyService?.CurrentGesture ??
            GlobalHotkeyService.NormalizeGesture(
                SettingsService.Settings.SearchHotkeyModifiers,
                SettingsService.Settings.SearchHotkeyKey);
        ShortcutNativeDiagnosticState shortcutNative =
            ShortcutNativeBackend.CaptureDiagnosticState();

        return new DeskBoxWhiteDiagnosticSnapshot(
            SchemaVersion: 6,
            GeneratedAtUtc: DateTimeOffset.UtcNow,
            AppVersion: GetDiagnosticVersion(),
            DistributionChannel: DistributionService.ChannelName,
            IsPackaged: DistributionService.IsPackaged,
            OperatingSystem: RuntimeInformation.OSDescription,
            ProcessArchitecture: RuntimeInformation.ProcessArchitecture.ToString(),
            UiCulture: CultureInfo.CurrentUICulture.Name,
            Hotkeys: new DeskBoxWhiteHotkeyDiagnostic(
                SettingsService.Settings.GlobalHotkeyEnabled,
                GlobalHotkeyService?.IsRegistered == true,
                (int)toggleGesture.Modifiers,
                toggleGesture.VirtualKey,
                GlobalHotkeyService?.ReceivedCount ?? 0,
                GlobalHotkeyService?.InvocationCount ?? 0,
                GlobalHotkeyService?.DispatchFailureCount ?? 0,
                GlobalHotkeyService?.UsesReservedHook == true,
                GlobalHotkeyService?.ReservedHookThreadId ?? 0,
                GlobalHotkeyService?.ReservedHookLastErrorCode ?? 0,
                GlobalHotkeyService?.ReservedHookTriggerCount ?? 0,
                GlobalHotkeyService?.ReservedHookPostFailureCount ?? 0,
                GlobalHotkeyService?.ReservedHookInputFailureCount ?? 0,
                !string.IsNullOrWhiteSpace(GlobalHotkeyService?.LastError),
                SettingsService.Settings.SearchHotkeyEnabled,
                SearchHotkeyService?.IsRegistered == true,
                (int)searchGesture.Modifiers,
                searchGesture.VirtualKey),
            Settings: new DeskBoxWhiteSettingsDiagnostic(
                SettingsService.LastLoadRecoveryState,
                SettingsService.HasPendingSave,
                saveFailure?.Operation,
                saveFailure?.OccurredAt.ToUniversalTime()),
            ShortcutNative: new DeskBoxWhiteShortcutNativeDiagnostic(
                shortcutNative.SelectedBackend,
                shortcutNative.ModuleName,
                shortcutNative.ModuleExists,
                shortcutNative.ModuleArchitecture,
                shortcutNative.ModuleSha256,
                shortcutNative.LoadAttempted,
                shortcutNative.LoadState,
                shortcutNative.AbiVersion,
                shortcutNative.Capabilities),
            RuntimeHealth: runtimeHealth,
            WidgetManager: widgetManager,
            Displays: displays,
            StorageMigration: WidgetManager?.StorageMigrationDiagnostic);
    }

    private static IReadOnlyList<DeskBoxWhiteDisplayDiagnostic> GetDisplayDiagnostics()
    {
        try
        {
            return Win32Helper.GetMonitorWorkAreaInfos()
                .Select((display, index) => new DeskBoxWhiteDisplayDiagnostic(
                    index + 1,
                    display.IsPrimary,
                    display.DpiScale,
                    ToDiagnosticRect(display.Monitor),
                    ToDiagnosticRect(display.WorkArea)))
                .ToArray();
        }
        catch (Exception ex)
        {
            Log($"[DiagnosticsBundle] Display enumeration failed: {ex.Message}");
            return [];
        }
    }

    private static DeskBoxWhiteDiagnosticRect ToDiagnosticRect(Win32Helper.RECT bounds)
    {
        return new DeskBoxWhiteDiagnosticRect(
            bounds.Left,
            bounds.Top,
            bounds.Right - bounds.Left,
            bounds.Bottom - bounds.Top);
    }

    private static string GetDiagnosticVersion()
    {
        Assembly assembly = typeof(App).Assembly;
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                   ?.InformationalVersion
               ?? assembly.GetName().Version?.ToString()
               ?? "unknown";
    }
}
