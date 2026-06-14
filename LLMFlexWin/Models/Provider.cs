namespace LLMFlexWin.Models;

public static class Provider
{
    public const string OpenAi = "openai";
    public const string OpenRouter = "openrouter";
    public const string Anthropic = "anthropic";
    public const string LmStudio = "lm_studio";
    public const string OpenAiCompatible = "openai_compatible";
    public const string Custom = "custom";

    public static string DefaultBaseUrl(string provider)
    {
        return provider.ToLowerInvariant() switch
        {
            OpenAi => "https://api.openai.com/v1",
            OpenRouter => "https://openrouter.ai/api/v1",
            Anthropic => "https://api.anthropic.com/v1",
            LmStudio => "http://localhost:1234/v1",
            _ => ""
        };
    }
}
