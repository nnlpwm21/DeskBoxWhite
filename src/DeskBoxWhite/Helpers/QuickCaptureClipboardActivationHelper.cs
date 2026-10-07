using DeskBoxWhite.Models;
using DeskBoxWhite.Services;
using Microsoft.UI.Xaml;

namespace DeskBoxWhite.Helpers;

public static class QuickCaptureClipboardActivationHelper
{
    public static async Task<bool> EnableAsync(XamlRoot? xamlRoot, LocalizationService localizationService)
    {
        await App.Current.QuickCaptureSettings.EnableClipboardFromOpenWidgetAsync();
        App.Log("[QuickCaptureClipboard] Enabled");
        return true;
    }
}
