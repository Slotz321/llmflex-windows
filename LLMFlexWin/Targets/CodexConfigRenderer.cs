using LLMFlexWin.Models;

namespace LLMFlexWin.Targets;

public static class CodexConfigRenderer
{
    public const string BlockStartMarker = "# === LLM Flex managed block START ===";
    public const string BlockEndMarker = "# === LLM Flex managed block END ===";
    public const string ProviderKey = "llmflex";
    public const string EnvKey = "OPENAI_API_KEY";
    public const string WireApi = "responses";

    public static string RenderBlock(Profile profile)
    {
        var lines = new List<string>
        {
            BlockStartMarker,
            $"model_provider = \"{ProviderKey}\""
        };

        if (!string.IsNullOrWhiteSpace(profile.ModelName))
        {
            lines.Add($"model = \"{EscapeToml(profile.ModelName)}\"");
        }

        lines.Add("");
        lines.Add($"[model_providers.{ProviderKey}]");
        lines.Add($"name = \"LLM Flex ({EscapeToml(profile.Name)})\"");
        lines.Add($"base_url = \"{EscapeToml(profile.BaseUrl)}\"");
        lines.Add($"env_key = \"{EnvKey}\"");
        lines.Add($"wire_api = \"{WireApi}\"");
        lines.Add(BlockEndMarker);

        return string.Join(Environment.NewLine, lines);
    }

    private static string EscapeToml(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }
}
