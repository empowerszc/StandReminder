using System.Text.Json;
using System.Text.Json.Serialization;

namespace StandReminder;

public class AppSettings
{
    public int ReminderIntervalMinutes { get; set; } = 60;
    public int StandDurationMinutes { get; set; } = 2;
    public bool AutostartEnabled { get; set; } = false;
    public int SnoozeMinutes { get; set; } = 5;

    private static string? _testSettingsDir;
    private static string? _testSettingsPath;

    internal static void SetTestPaths(string dir, string path)
    {
        _testSettingsDir = dir;
        _testSettingsPath = path;
    }

    internal static void ResetTestPaths()
    {
        _testSettingsDir = null;
        _testSettingsPath = null;
    }

    private static string SettingsDir => _testSettingsDir ?? Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "StandReminder");

    private static string SettingsPath => _testSettingsPath ?? Path.Combine(SettingsDir, "settings.json");

    public static AppSettings Load()
    {
        if (!File.Exists(SettingsPath))
        {
            return new AppSettings();
        }

        try
        {
            var json = File.ReadAllText(SettingsPath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json);
            return settings ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(SettingsDir);
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(SettingsPath, json);
    }
}
