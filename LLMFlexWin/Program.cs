using LLMFlexWin.Models;
using LLMFlexWin.Storage;

var profileStore = new ProfileStore();
var snapshotStore = new SnapshotStore();

var codexConfigPath = Path.Combine(UserHome(), ".codex", "config.toml");
var codexAuthPath = Path.Combine(UserHome(), ".codex", "auth.json");

var command = args.Length > 0 ? args[0].ToLowerInvariant() : "help";

switch (command)
{
    case "status":
        Console.WriteLine("LLMFlexWin status");
        Console.WriteLine($"Codex config:   {PathStatus(codexConfigPath)}");
        Console.WriteLine($"Codex auth:     {PathStatus(codexAuthPath)}");
        Console.WriteLine($"Profiles:       {profileStore.Count()}");
        Console.WriteLine($"Snapshots:      {snapshotStore.Count()}");
        Console.WriteLine($"Profile store:  {profileStore.ProfilesPath}");
        Console.WriteLine($"Snapshot store: {snapshotStore.SnapshotRoot}");
        break;

    case "list":
        var profiles = profileStore.All();

        if (profiles.Count == 0)
        {
            Console.WriteLine("Profiles: none yet");
            break;
        }

        Console.WriteLine("Profiles:");
        foreach (var profile in profiles)
        {
            PrintProfile(profile, "- ");
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

        profileStore.Upsert(newProfile);
        Console.WriteLine($"Saved profile: {newProfile.Name}");
        break;

    case "apply":
        var profileName = args.Length > 1 ? string.Join(" ", args[1..]) : "";
        if (string.IsNullOrWhiteSpace(profileName))
        {
            Console.WriteLine("Apply requires a profile name.");
            return;
        }

        var matched = profileStore.FindByName(profileName);
        if (matched is null)
        {
            Console.WriteLine($"Profile not found: {profileName}");
            return;
        }

        Console.WriteLine("Matched profile:");
        PrintProfile(matched, "");
        Console.WriteLine("Codex config write: not implemented yet");
        break;

    case "snapshot":
        Console.WriteLine("Capturing Codex snapshots if absent...");
        CaptureTarget(codexConfigPath, "codex");
        CaptureTarget(codexAuthPath, "codex");
        Console.WriteLine($"Snapshots: {snapshotStore.Count()}");
        break;

    case "restore":
        Console.WriteLine("Restoring Codex snapshots...");
        RestoreTarget(codexConfigPath, "codex");
        RestoreTarget(codexAuthPath, "codex");
        break;

    default:
        Console.WriteLine("LLMFlexWin commands:");
        Console.WriteLine("  status");
        Console.WriteLine("  list");
        Console.WriteLine("  add-profile <name> <provider> <baseUrl> <modelName>");
        Console.WriteLine("  apply <profileName>");
        Console.WriteLine("  snapshot");
        Console.WriteLine("  restore");
        break;
}

void CaptureTarget(string path, string tag)
{
    if (!File.Exists(path))
    {
        Console.WriteLine($"Skip missing: {path}");
        return;
    }

    var captured = snapshotStore.CaptureIfAbsent(path, tag);
    Console.WriteLine(captured
        ? $"Captured: {path}"
        : $"Snapshot already exists: {path}");
}

void RestoreTarget(string path, string tag)
{
    var restored = snapshotStore.Restore(path, tag);
    Console.WriteLine(restored
        ? $"Restored: {path}"
        : $"No snapshot: {path}");
}

static void PrintProfile(Profile profile, string prefix)
{
    Console.WriteLine($"{prefix}{profile.Name}");
    Console.WriteLine($"  Provider: {profile.Provider}");
    Console.WriteLine($"  Base URL: {profile.BaseUrl}");
    Console.WriteLine($"  Model:    {profile.ModelName}");
}

static string UserHome()
{
    return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
}

static string PathStatus(string path)
{
    return File.Exists(path) ? $"found: {path}" : $"missing: {path}";
}
