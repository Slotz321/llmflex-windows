using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using LLMFlexWin.Storage;

namespace LLMFlexWin.Targets;

public sealed class CodexAuthWriter
{
    private readonly string _authPath;
    private readonly SnapshotStore _snapshotStore;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public CodexAuthWriter(string authPath, SnapshotStore snapshotStore)
    {
        _authPath = authPath;
        _snapshotStore = snapshotStore;
    }

    public string Apply(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key cannot be empty.", nameof(apiKey));
        }

        var directory = Path.GetDirectoryName(_authPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _snapshotStore.CaptureIfAbsent(_authPath, "codex");

        var root = ReadExistingObject();

        root["OPENAI_API_KEY"] = apiKey;
        root["auth_mode"] = "apikey";

        var json = root.ToJsonString(JsonOptions);
        File.WriteAllText(_authPath, json + Environment.NewLine, new UTF8Encoding(false));

        return _authPath;
    }

    private JsonObject ReadExistingObject()
    {
        if (!File.Exists(_authPath))
        {
            return new JsonObject();
        }

        try
        {
            var json = File.ReadAllText(_authPath);
            var node = JsonNode.Parse(json);
            return node as JsonObject ?? new JsonObject();
        }
        catch
        {
            return new JsonObject();
        }
    }
}