using System.Text.Json;

namespace LLMFlexWin.Storage;

public sealed class LocalSecretStore : ISecretStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public string StoreDirectory { get; }
    public string SecretsPath { get; }

    public LocalSecretStore()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        StoreDirectory = Path.Combine(appData, "LLMFlexWindows");
        SecretsPath = Path.Combine(StoreDirectory, "secrets.local.json");
    }

    public void SetKey(Guid profileId, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key cannot be empty.", nameof(apiKey));
        }

        var secrets = ReadAll();
        secrets[profileId.ToString()] = apiKey;
        WriteAll(secrets);
    }

    public string? GetKey(Guid profileId)
    {
        var secrets = ReadAll();
        return secrets.TryGetValue(profileId.ToString(), out var key) ? key : null;
    }

    public bool HasKey(Guid profileId)
    {
        return !string.IsNullOrWhiteSpace(GetKey(profileId));
    }

    public void DeleteKey(Guid profileId)
    {
        var secrets = ReadAll();
        if (secrets.Remove(profileId.ToString()))
        {
            WriteAll(secrets);
        }
    }

    private Dictionary<string, string> ReadAll()
    {
        Directory.CreateDirectory(StoreDirectory);

        if (!File.Exists(SecretsPath))
        {
            return [];
        }

        try
        {
            var json = File.ReadAllText(SecretsPath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private void WriteAll(Dictionary<string, string> secrets)
    {
        Directory.CreateDirectory(StoreDirectory);
        var json = JsonSerializer.Serialize(secrets, JsonOptions);
        File.WriteAllText(SecretsPath, json);
    }
}