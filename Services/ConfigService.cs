using System.Text.Json;

public static class ConfigService
{
    private static readonly string FilePath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

    public static UserPreferences LoadPreferences()
    {
        if (!File.Exists(FilePath))
            return new UserPreferences();

        string json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<UserPreferences>(json)
               ?? new UserPreferences();
    }

    public static void SavePreferences(UserPreferences prefs)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(FilePath, JsonSerializer.Serialize(prefs, options));
    }
}

