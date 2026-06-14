using System.Text;
using LLMFlexWin.Models;
using LLMFlexWin.Storage;

namespace LLMFlexWin.Targets;

public sealed class CodexConfigWriter
{
    private readonly string _configPath;
    private readonly SnapshotStore _snapshotStore;

    public CodexConfigWriter(string configPath, SnapshotStore snapshotStore)
    {
        _configPath = configPath;
        _snapshotStore = snapshotStore;
    }

    public string Apply(Profile profile)
    {
        var directory = Path.GetDirectoryName(_configPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _snapshotStore.CaptureIfAbsent(_configPath, "codex");

        var existing = File.Exists(_configPath)
            ? File.ReadAllText(_configPath)
            : "";

        var managedBlock = CodexConfigRenderer.RenderBlock(profile);
        var updated = UpsertManagedBlock(existing, managedBlock);

        File.WriteAllText(_configPath, updated, new UTF8Encoding(false));

        return _configPath;
    }

    public static string UpsertManagedBlock(string existing, string managedBlock)
    {
        existing ??= "";

        var withoutManagedBlock = StripManagedBlock(existing);
        var withoutOldRootKeys = StripTopLevelKeys(withoutManagedBlock, "model", "model_provider");

        var firstTableIndex = FindFirstTableHeaderIndex(withoutOldRootKeys);

        if (firstTableIndex < 0)
        {
            return JoinSections(withoutOldRootKeys.Trim(), managedBlock);
        }

        var rootSection = withoutOldRootKeys[..firstTableIndex].Trim();
        var tableSections = withoutOldRootKeys[firstTableIndex..].Trim();

        return JoinSections(rootSection, managedBlock, tableSections);
    }

    private static string StripManagedBlock(string source)
    {
        var start = CodexConfigRenderer.BlockStartMarker;
        var end = CodexConfigRenderer.BlockEndMarker;

        while (true)
        {
            var startIndex = source.IndexOf(start, StringComparison.Ordinal);
            if (startIndex < 0) return source;

            var endIndex = source.IndexOf(end, startIndex, StringComparison.Ordinal);
            if (endIndex < 0) return source;

            endIndex += end.Length;

            while (endIndex < source.Length &&
                   (source[endIndex] == '\r' || source[endIndex] == '\n'))
            {
                endIndex++;
            }

            source = source[..startIndex] + source[endIndex..];
        }
    }

    private static string StripTopLevelKeys(string source, params string[] keys)
    {
        var lines = source.Replace("\r\n", "\n").Split('\n');
        var output = new List<string>();
        var inTable = false;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (IsTableHeader(trimmed))
            {
                inTable = true;
                output.Add(line);
                continue;
            }

            if (!inTable && keys.Any(key => IsKeyLine(trimmed, key)))
            {
                continue;
            }

            output.Add(line);
        }

        return string.Join(Environment.NewLine, output);
    }

    private static int FindFirstTableHeaderIndex(string source)
    {
        using var reader = new StringReader(source);
        var offset = 0;

        while (true)
        {
            var line = reader.ReadLine();
            if (line is null) return -1;

            if (IsTableHeader(line.Trim()))
            {
                return offset;
            }

            offset += line.Length + Environment.NewLine.Length;
        }
    }

    private static bool IsTableHeader(string trimmed)
    {
        return trimmed.StartsWith("[") && trimmed.EndsWith("]");
    }

    private static bool IsKeyLine(string trimmed, string key)
    {
        return trimmed.StartsWith(key, StringComparison.Ordinal)
            && trimmed[key.Length..].TrimStart().StartsWith("=", StringComparison.Ordinal);
    }

    private static string JoinSections(params string[] sections)
    {
        var nonEmpty = sections
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .ToArray();

        return string.Join(Environment.NewLine + Environment.NewLine, nonEmpty)
            + Environment.NewLine;
    }
}