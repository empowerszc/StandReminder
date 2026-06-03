// StandReminder/ReminderForm.cs
using System.Drawing;
using System.Windows.Forms;

namespace StandReminder;

public partial class ReminderForm : Form
{
    private int _standDurationMinutes;

    public ReminderForm(int standDurationMinutes)
    {
        _standDurationMinutes = standDurationMinutes;
        InitializeComponent();
        ConfigureForm();
    }

    private void ConfigureForm()
    {
        messageLabel.Text = $"站起来活动一下\n({_standDurationMinutes}分钟)";
        messageLabel.Font = new Font(messageLabel.Font.FontFamily, 11, FontStyle.Regular);

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
