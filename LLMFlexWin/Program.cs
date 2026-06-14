using LLMFlexWin.Models;
using LLMFlexWin.Storage;

var store = new ProfileStore();
var command = args.Length > 0 ? args[0].ToLowerInvariant() : "help";

switch (command)
{
    case "status":
        Console.WriteLine("LLMFlexWin status");
        Console.WriteLine($"Codex config:  {PathStatus(Path.Combine(UserHome(), ".codex", "config.toml"))}");
        Console.WriteLine($"Codex auth:    {PathStatus(Path.Combine(UserHome(), ".codex", "auth.json"))}");
        Console.WriteLine($"Claude config: {PathStatus(Path.Combine(UserHome(), ".claude", "settings.json"))}");
        Console.WriteLine($"Profiles:      {store.Count()}");
        Console.WriteLine($"Profile store: {store.ProfilesPath}");
        break;

    case "list":
        var profiles = store.All();

        if (profiles.Count == 0)
        {
            Console.WriteLine("Profiles: none yet");
            break;
        }

        Console.WriteLine("Profiles:");
        foreach (var profile in profiles)
        {
            Console.WriteLine($"- {profile.Name}");
            Console.WriteLine($"  Provider: {profile.Provider}");
            Console.WriteLine($"  Base URL: {profile.BaseUrl}");
            Console.WriteLine($"  Model:    {profile.ModelName}");
        }
        break;

    case "add-profile":
        if (args.Length < 5)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  add-profile <name> <provider> <baseUrl> <modelName>");
            return;
        }

        var newProfile = new Profile
        {
            Name = args[1],
            Provider = args[2],
            BaseUrl = args[3],
            ModelName = args[4]
        };

        store.Upsert(newProfile);
        Console.WriteLine($"Saved profile: {newProfile.Name}");
        break;

    case "apply":
        var profileName = args.Length > 1 ? string.Join(" ", args[1..]) : "";
        if (string.IsNullOrWhiteSpace(profileName))
        {
            Console.WriteLine("Apply requires a profile name.");
            return;
        }

        var matched = store.FindByName(profileName);
        if (matched is null)
        {
            Console.WriteLine($"Profile not found: {profileName}");
            return;
        }

        Console.WriteLine($"Matched profile: {matched.Name}");
        Console.WriteLine($"Provider:        {matched.Provider}");
        Console.WriteLine($"Base URL:        {matched.BaseUrl}");
        Console.WriteLine($"Model:           {matched.ModelName}");
        Console.WriteLine("Config write:    not implemented yet");
        break;

    case "restore":
        Console.WriteLine("Restore: not implemented yet");
        break;

    default:
        Console.WriteLine("LLMFlexWin commands:");
        Console.WriteLine("  status");
        Console.WriteLine("  list");
        Console.WriteLine("  add-profile <name> <provider> <baseUrl> <modelName>");
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
