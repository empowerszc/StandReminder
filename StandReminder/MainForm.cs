// StandReminder/MainForm.cs
using System.Windows.Forms;

namespace StandReminder;

public partial class MainForm : Form
{
    private AppSettings _settings = null!;

    // Power broadcast constants
    private const int WM_POWERBROADCAST = 0x0218;
    private const int PBT_APMSUSPEND = 0x0004;
    private const int PBT_APMRESUMEAUTOMATIC = 0x0012;

    public MainForm()
    {
        InitializeComponent();
        LoadSettings();
        this.Visible = false;
        this.ShowInTaskbar = false;
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
        trayIcon.Visible = false;
        Application.Exit();
    }

    protected override void SetVisibleCore(bool value)
    {
        base.SetVisibleCore(false); // Always keep hidden
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_POWERBROADCAST)
        {
            switch (m.WParam.ToInt32())
            {
                case PBT_APMSUSPEND:
                    // System entering sleep/hibernate - stop timer
                    reminderTimer.Stop();
                    break;

                case PBT_APMRESUMEAUTOMATIC:
                    // System resumed from sleep - reset timer
                    reminderTimer.Stop();
                    reminderTimer.Interval = _settings.ReminderIntervalMinutes * 60 * 1000;
                    reminderTimer.Start();
                    break;
            }
        }

        base.WndProc(ref m);
    }
}
