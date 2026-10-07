using DeskBoxWhite.Models;
using DeskBoxWhite.ViewModels;

namespace DeskBoxWhite.Services;

internal static class QuickCaptureClipboardCopyPolicy
{
    public static bool ShouldCopyBitmap(QuickCaptureItemViewModel item) =>
        item.IsRecent && item.Type == QuickCaptureItemType.Image;
}
