using DeskBoxWhite.Services;

namespace DeskBoxWhite.Tests;

public sealed class ActiveDeskBoxWhiteDragRegistryTests : IDisposable
{
    private readonly string _root;

    public ActiveDeskBoxWhiteDragRegistryTests()
    {
        _root = Path.Combine(
            Path.GetTempPath(),
            "deskboxwhite-active-drag-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    [Fact]
    public void TryMatch_RecognizesAllPathsFromAnActiveDrag()
    {
        string first = Path.Combine(_root, "a.txt");
        string second = Path.Combine(_root, "b.txt");
        DateTime now = DateTime.UtcNow;
        ActiveDeskBoxWhiteDragRegistry.Begin(
            "s1",
            "widget-1",
            [first, second],
            fromStackPopover: false,
            utcNow: now);
        try
        {
            Assert.True(ActiveDeskBoxWhiteDragRegistry.TryMatch(
                [first, second],
                out string widgetId,
                out bool fromPopover,
                utcNow: now));
            Assert.Equal("widget-1", widgetId);
            Assert.False(fromPopover);
            // A subset of the dragged paths still identifies the same drag.
            Assert.True(ActiveDeskBoxWhiteDragRegistry.TryMatch(
                [second],
                out _,
                out _,
                utcNow: now));
        }
        finally
        {
            ActiveDeskBoxWhiteDragRegistry.End("s1");
        }
    }

    [Fact]
    public void TryMatch_RejectsForeignAndMixedPayloads()
    {
        string ours = Path.Combine(_root, "ours.txt");
        string theirs = Path.Combine(_root, "theirs.txt");
        DateTime now = DateTime.UtcNow;
        ActiveDeskBoxWhiteDragRegistry.Begin(
            "s2",
            "widget-2",
            [ours],
            fromStackPopover: true,
            utcNow: now);
        try
        {
            // A foreign path is not a self-drag.
            Assert.False(ActiveDeskBoxWhiteDragRegistry.TryMatch(
                [theirs],
                out _,
                out _,
                utcNow: now));
            // A mixed payload (ours + theirs) cannot be proven self-originated.
            Assert.False(ActiveDeskBoxWhiteDragRegistry.TryMatch(
                [ours, theirs],
                out _,
                out _,
                utcNow: now));
            // Popover provenance survives the match.
            Assert.True(ActiveDeskBoxWhiteDragRegistry.TryMatch(
                [ours],
                out _,
                out bool fromPopover,
                utcNow: now));
            Assert.True(fromPopover);
        }
        finally
        {
            ActiveDeskBoxWhiteDragRegistry.End("s2");
        }
    }

    [Fact]
    public void TryMatch_DoesNotMatchAfterEnd()
    {
        string path = Path.Combine(_root, "ended.txt");
        ActiveDeskBoxWhiteDragRegistry.Begin(
            "s3",
            "widget-3",
            [path],
            fromStackPopover: false);
        ActiveDeskBoxWhiteDragRegistry.End("s3");
        Assert.False(ActiveDeskBoxWhiteDragRegistry.TryMatch(
            [path],
            out _,
            out _));
    }

    [Fact]
    public void TryMatch_ExpiresStaleEntries()
    {
        string path = Path.Combine(_root, "stale.txt");
        DateTime registered = DateTime.UtcNow - TimeSpan.FromMinutes(20);
        DateTime now = DateTime.UtcNow;
        // Begin prunes on write: register via a backdated clock.
        ActiveDeskBoxWhiteDragRegistry.Begin(
            "s4",
            "widget-4",
            [path],
            fromStackPopover: false,
            utcNow: registered);
        try
        {
            Assert.False(ActiveDeskBoxWhiteDragRegistry.TryMatch(
                [path],
                out _,
                out _,
                utcNow: now));
        }
        finally
        {
            ActiveDeskBoxWhiteDragRegistry.End("s4");
        }
    }

    [Fact]
    public void TryMatch_NormalizesEquivalentPathForms()
    {
        string path = Path.Combine(_root, "case.TXT");
        DateTime now = DateTime.UtcNow;
        ActiveDeskBoxWhiteDragRegistry.Begin(
            "s5",
            "widget-5",
            [path],
            fromStackPopover: false,
            utcNow: now);
        try
        {
            Assert.True(ActiveDeskBoxWhiteDragRegistry.TryMatch(
                [path.ToUpperInvariant()],
                out _,
                out _,
                utcNow: now));
            Assert.True(ActiveDeskBoxWhiteDragRegistry.TryMatch(
                [Path.Combine(_root, ".", "case.TXT")],
                out _,
                out _,
                utcNow: now));
        }
        finally
        {
            ActiveDeskBoxWhiteDragRegistry.End("s5");
        }
    }

    public void Dispose()
    {
        try { Directory.Delete(_root, recursive: true); } catch { }
    }
}
