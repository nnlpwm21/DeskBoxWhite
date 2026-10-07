using DeskBoxWhite.Contracts;
using DeskBoxWhite.Controls.WidgetContents;
using DeskBoxWhite.Models;

namespace DeskBoxWhite.Services;

internal sealed class GlanceWidgetContentProvider : IWidgetContentProvider
{
    public WidgetKind WidgetKind => WidgetKind.Glance;
    public bool CanCreateDetachedContent => true;

    public IWidgetContent CreateDetachedContent(WidgetConfig config, WidgetContentProviderContext context)
    {
        return new GlanceWidgetContentAdapter(
            config,
            context.LocalizationService,
            settingsService: context.SettingsService);
    }
}
