using DeskBoxWhite.Services;

namespace DeskBoxWhite.ViewModels;

public sealed partial class SettingsViewModel
{
    public void RefreshManagedStorageState()
    {
        ManagedStorageRootPath = SettingsService.NormalizeManagedStorageRootPath(
            _settingsService.Settings.DefaultManagedStorageRootPath);
    }
}
