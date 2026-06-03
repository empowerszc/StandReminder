// StandReminder/MainForm.cs
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
        if (this.InvokeRequired)
        {
            this.Invoke(ShowReminder);
            return;
        }

        var reminderForm = new ReminderForm(_settings.StandDurationMinutes);
        reminderForm.Closed += (s, args) =>
        {
            reminderTimer.Interval = _settings.ReminderIntervalMinutes * 60 * 1000;
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
}
