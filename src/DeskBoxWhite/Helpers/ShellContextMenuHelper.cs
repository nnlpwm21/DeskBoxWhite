using DeskBoxWhite.Platform;
#if DESKBOXWHITE_NATIVE_AOT && DESKBOXWHITE_AOT_SMOKE_HARNESS
using DeskBoxWhite.Services;
#endif

namespace DeskBoxWhite.Helpers;

/// <summary>
/// Invokes Shell-owned property UI for a file or folder. Explorer context menus
/// are intentionally handled by <see cref="ShellContextMenuProxy"/> so native
/// menu extensions never load into the DeskBoxWhite process.
/// </summary>
public static class ShellContextMenuHelper
{
    private const uint SHOP_FILEPATH = 0x2;

    /// <summary>
    /// Shows the native properties dialog for a file or folder.
    /// </summary>
    public static bool ShowProperties(IntPtr hwnd, string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

#if DESKBOXWHITE_NATIVE_AOT && DESKBOXWHITE_AOT_SMOKE_HARNESS
        bool trackedInvocation =
            AotFilePropertiesFixture.TryBeginInvocation(hwnd, filePath);
#endif
        try
        {
            bool invoked = Shell32NativeMethods.SHObjectProperties(
                hwnd,
                SHOP_FILEPATH,
                filePath,
                null);
#if DESKBOXWHITE_NATIVE_AOT && DESKBOXWHITE_AOT_SMOKE_HARNESS
            if (trackedInvocation)
            {
                AotFilePropertiesFixture.RecordInvocationResult(
                    invoked,
                    error: null);
            }
#endif
            return invoked;
        }
        catch (Exception ex)
        {
            _ = ex;
#if DESKBOXWHITE_NATIVE_AOT && DESKBOXWHITE_AOT_SMOKE_HARNESS
            if (trackedInvocation)
            {
                AotFilePropertiesFixture.RecordInvocationResult(
                    invoked: false,
                    ex.ToString());
            }
#endif
            throw;
        }
    }
}
