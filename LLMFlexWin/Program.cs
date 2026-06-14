using System;
using System.IO;

var command = args.Length > 0 ? args[0].ToLowerInvariant() : "help";

switch (command)
{
    case "status":
        Console.WriteLine("LLMFlexWin status");
        Console.WriteLine($"Codex config:  {PathStatus(Path.Combine(UserHome(), ".codex", "config.toml"))}");
        Console.WriteLine($"Codex auth:    {PathStatus(Path.Combine(UserHome(), ".codex", "auth.json"))}");
        Console.WriteLine($"Claude config: {PathStatus(Path.Combine(UserHome(), ".claude", "settings.json"))}");
        break;

    case "list":
        Console.WriteLine("Profiles: none yet");
        break;

    case "restore":
        Console.WriteLine("Restore: not implemented yet");
        break;

    case "apply":
        var profileName = args.Length > 1 ? string.Join(" ", args[1..]) : "";
        Console.WriteLine(!string.IsNullOrWhiteSpace(profileName)
            ? $"Apply profile: {profileName}"
            : "Apply requires a profile name.");
        break;

    default:
        Console.WriteLine("LLMFlexWin commands:");
        Console.WriteLine("  status");
        Console.WriteLine("  list");
        Console.WriteLine("  apply <profileName>");
        Console.WriteLine("  restore");
        break;
}

static string UserHome()
{
    return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
}

static string PathStatus(string path)
{
    return File.Exists(path) ? $"found: {path}" : $"missing: {path}";
}
