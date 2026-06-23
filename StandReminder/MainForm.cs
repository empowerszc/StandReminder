// StandReminder/MainForm.cs
using Microsoft.Win32;
using System.Windows.Forms;

namespace StandReminder;

public partial class MainForm : Form
{
    private AppSettings _settings = null!;
    private bool _isReminderOpen = false;
    private DateTime _lastReminderTime = DateTime.MinValue;

    public MainForm()
    {
        InitializeComponent();
        LoadSettings();
        this.Visible = false;
        this.ShowInTaskbar = false;

        // Listen to power mode changes (sleep/wake)
        SystemEvents.PowerModeChanged += OnPowerModeChanged;
        // Listen to session lock/unlock (screen lock, sleep, logon)
        SystemEvents.SessionSwitch += OnSessionSwitch;
    }

    private void LoadSettings()
    {
        _settings = AppSettings.Load();
        _lastReminderTime = DateTime.Now;
        reminderTimer.Interval = _settings.ReminderIntervalMinutes * 60 * 1000;
        reminderTimer.Start();
    }

    private void ReminderTimer_Tick(object? sender, EventArgs e)
    {
        ShowReminder();
    }

    private void ShowReminder()
    {
        // Prevent multiple reminders from stacking
        if (_isReminderOpen) return;

        // Verify enough time has actually passed since last reminder
        var elapsed = DateTime.Now - _lastReminderTime;
        if (elapsed.TotalMinutes < _settings.ReminderIntervalMinutes - 1)
        {
            return;
        }

        _lastReminderTime = DateTime.Now;
        _isReminderOpen = true;

        var reminderForm = new ReminderForm(_settings.StandDurationMinutes, _settings.SnoozeMinutes);
        reminderForm.Closed += (s, args) =>
        {
            _isReminderOpen = false;
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
        if (_isReminderOpen) return;
        _lastReminderTime = DateTime.Now;
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
        SystemEvents.SessionSwitch -= OnSessionSwitch;
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
                StopTimerForAway();
                break;

            case PowerModes.Resume:
                ResetTimerOnReturn();
                break;
        }
    }

    private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        switch (e.Reason)
        {
            case SessionSwitchReason.SessionLock:
                // Screen locked (Win+L, sleep, timeout)
                StopTimerForAway();
                break;

            case SessionSwitchReason.SessionUnlock:
                // User unlocked/logged back in
                ResetTimerOnReturn();
                break;
        }
    }

    private void StopTimerForAway()
    {
        reminderTimer.Stop();
    }

    private void ResetTimerOnReturn()
    {
        reminderTimer.Stop();
        _lastReminderTime = DateTime.Now;
        reminderTimer.Interval = _settings.ReminderIntervalMinutes * 60 * 1000;
        reminderTimer.Start();
    }
}
