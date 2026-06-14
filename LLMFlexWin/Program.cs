using LLMFlexWin.Models;
using LLMFlexWin.Storage;
using LLMFlexWin.Targets;

var profileStore = new ProfileStore();
var snapshotStore = new SnapshotStore();
var secretStore = new LocalSecretStore();

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
        Console.WriteLine($"Secret store:   {secretStore.SecretsPath}");
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
            Console.WriteLine($"  API key:  {(secretStore.HasKey(profile.Id) ? "present" : "missing")}");
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

    case "set-key":
        if (args.Length < 3)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  set-key <profileName> <apiKey>");
            return;
        }

        var setKeyProfileName = args[1];
        var apiKey = args[2];

        var setKeyProfile = profileStore.FindByName(setKeyProfileName);
        if (setKeyProfile is null)
        {
            Console.WriteLine($"Profile not found: {setKeyProfileName}");
            return;
        }

        secretStore.SetKey(setKeyProfile.Id, apiKey);
        Console.WriteLine($"Saved key for: {setKeyProfile.Name}");
        break;

    case "has-key":
        if (args.Length < 2)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  has-key <profileName>");
            return;
        }

        var hasKeyProfileName = args[1];

        var hasKeyProfile = profileStore.FindByName(hasKeyProfileName);
        if (hasKeyProfile is null)
        {
            Console.WriteLine($"Profile not found: {hasKeyProfileName}");
            return;
        }

        Console.WriteLine($"API key for {hasKeyProfile.Name}: {(secretStore.HasKey(hasKeyProfile.Id) ? "present" : "missing")}");
        break;

    case "apply":
        var applyProfile = FindProfileFromArgs(args, "Apply");
        if (applyProfile is null) return;

        Console.WriteLine("Matched profile:");
        PrintProfile(applyProfile, "");

        var writer = new CodexConfigWriter(codexConfigPath, snapshotStore);
        var writtenPath = writer.Apply(applyProfile);

        Console.WriteLine($"Codex config written: {writtenPath}");
        Console.WriteLine($"API key:              {(secretStore.HasKey(applyProfile.Id) ? "present" : "missing")}");
        Console.WriteLine("Codex auth write:     not implemented yet");
        break;

    case "preview":
        var previewProfile = FindProfileFromArgs(args, "Preview");
        if (previewProfile is null) return;

        Console.WriteLine($"Previewing Codex config for: {previewProfile.Name}");
        Console.WriteLine();
        Console.WriteLine(CodexConfigRenderer.RenderBlock(previewProfile));
        Console.WriteLine();
        Console.WriteLine("No files were modified.");
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
        Console.WriteLine("  set-key <profileName> <apiKey>");
        Console.WriteLine("  has-key <profileName>");
        Console.WriteLine("  apply <profileName>");
        Console.WriteLine("  preview <profileName>");
        Console.WriteLine("  snapshot");
        Console.WriteLine("  restore");
        break;
}

Profile? FindProfileFromArgs(string[] commandArgs, string commandName)
{
    var profileName = commandArgs.Length > 1 ? string.Join(" ", commandArgs[1..]) : "";
    if (string.IsNullOrWhiteSpace(profileName))
    {
        Console.WriteLine($"{commandName} requires a profile name.");
        return null;
    }

    var profile = profileStore.FindByName(profileName);
    if (profile is null)
    {
        Console.WriteLine($"Profile not found: {profileName}");
        return null;
    }

    return profile;
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