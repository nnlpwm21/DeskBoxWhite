using Microsoft.Extensions.DependencyInjection;

namespace DeskBoxWhite.Services;

/// <summary>
/// Central DI registration for all core DeskBoxWhite services.
/// These registrations have application lifetime. Feature/window factories
/// own shorter-lived instances; App disposes this container after its consumers.
/// </summary>
public static class ServiceRegistry
{
    /// <summary>
    /// Registers all core application services into the given service collection.
    /// </summary>
    public static IServiceCollection AddDeskBoxWhiteServices(this IServiceCollection services)
    {
        // ── Core infrastructure ──────────────────────────────────────────
        services.AddSingleton<SettingsService>();
        services.AddSingleton<DeskBoxWhiteDataBackupService>();
        services.AddSingleton<ICredentialStore>(_ => new PasswordVaultCredentialStore());
        services.AddSingleton<CloudBackupService>(sp =>
            new CloudBackupService(
                sp.GetRequiredService<DeskBoxWhiteDataBackupService>(),
                sp.GetRequiredService<SettingsService>(),
                sp.GetRequiredService<ICredentialStore>()));
        services.AddSingleton<DeskBoxWhiteAttachmentHealthService>();
        services.AddSingleton<DeskBoxWhiteDiagnosticsBundleService>();
        services.AddSingleton<FileService>();
        services.AddSingleton<ResizeGuideOverlayService>();
        services.AddSingleton<ManagedStorageDesktopShortcutService>();

        // ── Feature services ─────────────────────────────────────────────
        services.AddSingleton<OrganizerService>(sp =>
            new OrganizerService(
                sp.GetRequiredService<SettingsService>(),
                sp.GetRequiredService<FileService>()));
        services.AddSingleton<QuickCaptureService>(_ => new QuickCaptureService());
        services.AddSingleton<LocalizationService>();
        services.AddSingleton<ThemeService>();

        // ── Update (factory-based) ───────────────────────────────────────
        services.AddSingleton<IAppUpdateService>(_ =>
            AppUpdateServiceFactory.Create(AppDistributionService.Current));

        return services;
    }
}
