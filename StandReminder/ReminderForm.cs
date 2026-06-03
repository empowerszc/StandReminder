// StandReminder/ReminderForm.cs
using System.Drawing;
using System.Windows.Forms;

namespace StandReminder;

public partial class ReminderForm : Form
{
    private int _standDurationMinutes;
    private int _snoozeMinutes;

    public ReminderForm(int standDurationMinutes, int snoozeMinutes)
    {
        _standDurationMinutes = standDurationMinutes;
        _snoozeMinutes = snoozeMinutes;
        InitializeComponent();
        ConfigureForm();
    }

    private void ConfigureForm()
    {
        messageLabel.Text = $"站起来活动一下\n({_standDurationMinutes}分钟)";
        var oldFont = messageLabel.Font;
        messageLabel.Font = new Font(oldFont.FontFamily, 11, FontStyle.Regular);
        oldFont.Dispose();

        snoozeButton.Text = $"稍后({_snoozeMinutes}分钟)";

        PositionBottomRight();
    }

    private void PositionBottomRight()
    {
        var screen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
        var workingArea = screen.WorkingArea;

        this.Location = new Point(
            workingArea.Right - this.Width - 20,
            workingArea.Bottom - this.Height - 60
        );
    }

    protected override bool ShowWithoutActivation => true;

    private void CloseButton_Click(object? sender, EventArgs e)
    {
        this.Close();
    }

    private void SnoozeButton_Click(object? sender, EventArgs e)
    {
        this.Tag = "snooze";
        this.Close();
    }
}
