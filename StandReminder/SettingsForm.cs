// StandReminder/SettingsForm.cs
using Microsoft.Win32;
using System.Windows.Forms;

namespace StandReminder;

public partial class SettingsForm : Form
{
    private AppSettings _settings;
    private MainForm _mainForm;

    public SettingsForm(AppSettings settings, MainForm mainForm)
    {
        _settings = settings;
        _mainForm = mainForm;
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        intervalNumeric.Value = _settings.ReminderIntervalMinutes;
        durationNumeric.Value = _settings.StandDurationMinutes;
        autostartCheckBox.Checked = _settings.AutostartEnabled;
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        _settings.ReminderIntervalMinutes = (int)intervalNumeric.Value;
        _settings.StandDurationMinutes = (int)durationNumeric.Value;
        _settings.AutostartEnabled = autostartCheckBox.Checked;
        _settings.Save();

        UpdateAutostart();
        UpdateMainFormTimer();

        this.Close();
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        this.Close();
    }

    private void UpdateAutostart()
    {
        const string keyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
        const string valueName = "StandReminder";

        using var key = Registry.CurrentUser.OpenSubKey(keyName, true);
        if (key == null) return;

        if (_settings.AutostartEnabled)
        {
            key.SetValue(valueName, Application.ExecutablePath);
        }
        else
        {
            key.DeleteValue(valueName, false);
        }
    }

    private void UpdateMainFormTimer()
    {
        _mainForm.SnoozeReminder(_settings.ReminderIntervalMinutes);
    }
}
