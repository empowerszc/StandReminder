// StandReminder/MainForm.cs
using Microsoft.Win32;
using System.Windows.Forms;

namespace StandReminder;

public partial class MainForm : Form
{
    private AppSettings _settings = null!;

    public MainForm()
    {
        InitializeComponent();
        LoadSettings();
        this.Visible = false;
        this.ShowInTaskbar = false;

        // Subscribe to power mode changes
        SystemEvents.PowerModeChanged += OnPowerModeChanged;
    }

    private void LoadSettings()
    {
        _settings = AppSettings.Load();
        reminderTimer.Interval = _settings.ReminderIntervalMinutes * 60 * 1000;
        reminderTimer.Start();
    }

    private void ReminderTimer_Tick(object? sender, EventArgs e)
    {
        ShowReminder();
    }

    private void ShowReminder()
    {
        var reminderForm = new ReminderForm(_settings.StandDurationMinutes, _settings.SnoozeMinutes);
        reminderForm.Closed += (s, args) =>
        {
            reminderTimer.Stop();
            if (reminderForm.Tag?.ToString() == "snooze")
            {
                reminderTimer.Interval = _settings.SnoozeMinutes * 60 * 1000;
            }
            else
            {
                reminderTimer.Interval = _settings.ReminderIntervalMinutes * 60 * 1000;
            }
            reminderTimer.Start();
        };
        reminderForm.Show();
    }

    public void ShowReminderNow()
    {
        ShowReminder();
    }

    public void SnoozeReminder(int minutes)
    {
        reminderTimer.Stop();
        reminderTimer.Interval = minutes * 60 * 1000;
        reminderTimer.Start();
    }

    private void SettingsMenuItem_Click(object? sender, EventArgs e)
    {
        var settingsForm = new SettingsForm(_settings, this);
        settingsForm.ShowDialog();
    }

    private void RemindNowMenuItem_Click(object? sender, EventArgs e)
    {
        ShowReminderNow();
    }

    private void ExitMenuItem_Click(object? sender, EventArgs e)
    {
        SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        trayIcon.Visible = false;
        Application.Exit();
    }

    protected override void SetVisibleCore(bool value)
    {
        base.SetVisibleCore(false); // Always keep hidden
    }

    private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        switch (e.Mode)
        {
            case PowerModes.Suspend:
                // System entering sleep/hibernate - stop timer
                reminderTimer.Stop();
                break;

            case PowerModes.Resume:
                // System resumed from sleep - reset timer
                reminderTimer.Stop();
                reminderTimer.Interval = _settings.ReminderIntervalMinutes * 60 * 1000;
                reminderTimer.Start();
                break;
        }
    }
}
