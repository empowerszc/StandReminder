using StandReminder;
using Xunit;

namespace StandReminder.Tests;

public class AppSettingsTests : IDisposable
{
    private readonly string _testDir;
    private readonly string _testPath;

    public AppSettingsTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), "StandReminderTests_" + Guid.NewGuid().ToString("N"));
        _testPath = Path.Combine(_testDir, "settings.json");
        AppSettings.SetTestPaths(_testDir, _testPath);
    }

    public void Dispose()
    {
        AppSettings.ResetTestPaths();
        if (File.Exists(_testPath))
        {
            File.Delete(_testPath);
        }
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    [Fact]
    public void Load_ReturnsDefaults_WhenFileNotExists()
    {
        var settings = AppSettings.Load();
        Assert.Equal(60, settings.ReminderIntervalMinutes);
        Assert.Equal(2, settings.StandDurationMinutes);
        Assert.False(settings.AutostartEnabled);
        Assert.Equal(5, settings.SnoozeMinutes);
    }

    [Fact]
    public void SaveAndLoad_PersistsValues()
    {
        var original = new AppSettings
        {
            ReminderIntervalMinutes = 30,
            StandDurationMinutes = 5,
            AutostartEnabled = true,
            SnoozeMinutes = 10
        };
        original.Save();

        var loaded = AppSettings.Load();
        Assert.Equal(30, loaded.ReminderIntervalMinutes);
        Assert.Equal(5, loaded.StandDurationMinutes);
        Assert.True(loaded.AutostartEnabled);
        Assert.Equal(10, loaded.SnoozeMinutes);
    }
}
