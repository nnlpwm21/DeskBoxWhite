using DeskBoxWhite.Services;
using Windows.ApplicationModel.DataTransfer;

namespace DeskBoxWhite.Tests;

public sealed class DeskBoxWhiteDragDataTests
{
    [Fact]
    public async Task InternalMixedFileAndFolderDrop_PreservesEveryPath()
    {
        string tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "DeskBoxWhite.Tests",
            Guid.NewGuid().ToString("N"));
        string folderPath = Directory.CreateDirectory(
            Path.Combine(tempDirectory, "folder")).FullName;
        string filePath = Path.Combine(tempDirectory, "file.txt");
        File.WriteAllText(filePath, "content");

        try
        {
            var dataPackage = new DataPackage();
            dataPackage.Properties[DeskBoxWhiteDragData.SourcePathsProperty] =
                new[] { filePath, folderPath };

            IReadOnlyList<DroppedFilePath> internalFiles =
                DeskBoxWhiteDragData.GetInternalDroppedFiles(dataPackage.GetView());
            using DroppedFileBatch batch =
                await DeskBoxWhiteDragData.TryGetDroppedFilesAsync(dataPackage.GetView());

            Assert.Equal([filePath, folderPath], internalFiles.Select(file => file.Path));
            Assert.Equal([filePath, folderPath], batch.Files.Select(file => file.Path));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }
}
