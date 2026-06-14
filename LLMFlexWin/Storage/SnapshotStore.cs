using System.Security.Cryptography;
using System.Text;

namespace LLMFlexWin.Storage;

public sealed class SnapshotStore
{
    public string SnapshotRoot { get; }

    public SnapshotStore()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        SnapshotRoot = Path.Combine(appData, "LLMFlexWindows", "snapshots");
    }

    public bool CaptureIfAbsent(string sourcePath, string tag)
    {
        if (!File.Exists(sourcePath))
        {
            return false;
        }

        var snapshotPath = GetSnapshotPath(sourcePath, tag);
        if (File.Exists(snapshotPath))
        {
            return false;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(snapshotPath)!);
        File.Copy(sourcePath, snapshotPath, overwrite: false);
        return true;
    }

    public bool Restore(string targetPath, string tag)
    {
        var snapshotPath = GetSnapshotPath(targetPath, tag);
        if (!File.Exists(snapshotPath))
        {
            return false;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        File.Copy(snapshotPath, targetPath, overwrite: true);
        return true;
    }

    public int Count()
    {
        if (!Directory.Exists(SnapshotRoot))
        {
            return 0;
        }

        return Directory.GetFiles(SnapshotRoot, "*.bak", SearchOption.AllDirectories).Length;
    }

    public string GetSnapshotPath(string sourcePath, string tag)
    {
        var fullPath = Path.GetFullPath(sourcePath);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(fullPath))).ToLowerInvariant();
        var fileName = Path.GetFileName(sourcePath);
        return Path.Combine(SnapshotRoot, tag, $"{fileName}.{hash}.bak");
    }
}
