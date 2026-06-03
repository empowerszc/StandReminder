// StandReminder/MainForm.Designer.cs
#nullable enable
namespace StandReminder;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;
    private System.Windows.Forms.NotifyIcon trayIcon = null!;
    private System.Windows.Forms.ContextMenuStrip trayMenu = null!;
    private System.Windows.Forms.ToolStripMenuItem settingsMenuItem = null!;
    private System.Windows.Forms.ToolStripMenuItem remindNowMenuItem = null!;
    private System.Windows.Forms.ToolStripSeparator separator = null!;
    private System.Windows.Forms.ToolStripMenuItem exitMenuItem = null!;
    private System.Windows.Forms.Timer reminderTimer = null!;
    private System.Drawing.Icon? _trayIconResource;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
            _trayIconResource?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
        this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.remindNowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.separator = new System.Windows.Forms.ToolStripSeparator();
        this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.reminderTimer = new System.Windows.Forms.Timer(this.components);

        this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsMenuItem,
            this.remindNowMenuItem,
            this.separator,
            this.exitMenuItem});

        this.settingsMenuItem.Text = "设置";
        this.settingsMenuItem.Click += new System.EventHandler(this.SettingsMenuItem_Click);

        this.remindNowMenuItem.Text = "立即提醒";
        this.remindNowMenuItem.Click += new System.EventHandler(this.RemindNowMenuItem_Click);

        this.exitMenuItem.Text = "退出";
        this.exitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);

        this.trayIcon.ContextMenuStrip = this.trayMenu;
        this.trayIcon.Text = "站立提醒";
        this.trayIcon.Visible = true;

        var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "tray.ico");
        if (File.Exists(iconPath))
        {
            _trayIconResource = new System.Drawing.Icon(iconPath);
            this.trayIcon.Icon = _trayIconResource;
        }

        this.reminderTimer.Tick += new System.EventHandler(this.ReminderTimer_Tick);

        this.components.Add(this.trayIcon);
        this.components.Add(this.trayMenu);
        this.components.Add(this.reminderTimer);
    }
}
