using System.Text.Json;
using LLMFlexWin.Models;

namespace LLMFlexWin.Storage;

public sealed class ProfileStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public string StoreDirectory { get; }
    public string ProfilesPath { get; }

    public ProfileStore()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        StoreDirectory = Path.Combine(appData, "LLMFlexWindows");
        ProfilesPath = Path.Combine(StoreDirectory, "profiles.json");
    }

    public List<Profile> All()
    {
        Directory.CreateDirectory(StoreDirectory);

        if (!File.Exists(ProfilesPath))
        {
            return [];
        }

        try
        {
            var json = File.ReadAllText(ProfilesPath);
            return JsonSerializer.Deserialize<List<Profile>>(json, JsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public void Upsert(Profile profile)
    {
        Directory.CreateDirectory(StoreDirectory);

        var profiles = All();
        var index = profiles.FindIndex(p =>
            p.Id == profile.Id ||
            string.Equals(p.Name, profile.Name, StringComparison.OrdinalIgnoreCase));

        if (index >= 0)
        {
            profile.Id = profiles[index].Id;
            profile.CreatedAt = profiles[index].CreatedAt;
            profiles[index] = profile;
        }
        else
        {
            profiles.Add(profile);
        }

        var json = JsonSerializer.Serialize(profiles, JsonOptions);
        File.WriteAllText(ProfilesPath, json);
    }

    public Profile? FindByName(string name)
    {
        return All().FirstOrDefault(p =>
            string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public int Count()
    {
        return All().Count;
    }
}
