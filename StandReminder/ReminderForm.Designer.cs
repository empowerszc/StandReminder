// StandReminder/ReminderForm.Designer.cs
namespace StandReminder;

partial class ReminderForm
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Label messageLabel = null!;
    private System.Windows.Forms.Button closeButton = null!;
    private System.Windows.Forms.Button snoozeButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.messageLabel = new System.Windows.Forms.Label();
        this.closeButton = new System.Windows.Forms.Button();
        this.snoozeButton = new System.Windows.Forms.Button();
        this.SuspendLayout();

        // messageLabel
        this.messageLabel.AutoSize = true;
        this.messageLabel.Location = new System.Drawing.Point(12, 15);
        this.messageLabel.Name = "messageLabel";
        this.messageLabel.Size = new System.Drawing.Size(0, 15);
        this.messageLabel.TabIndex = 0;

        // closeButton
        this.closeButton.Location = new System.Drawing.Point(45, 80);
        this.closeButton.Name = "closeButton";
        this.closeButton.Size = new System.Drawing.Size(75, 28);
        this.closeButton.TabIndex = 1;
        this.closeButton.Text = "关闭";
        this.closeButton.UseVisualStyleBackColor = true;
        this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);

        // snoozeButton
        this.snoozeButton.Location = new System.Drawing.Point(140, 80);
        this.snoozeButton.Name = "snoozeButton";
        this.snoozeButton.Size = new System.Drawing.Size(100, 28);
        this.snoozeButton.TabIndex = 2;
        this.snoozeButton.Text = "稍后(5分钟)";
        this.snoozeButton.UseVisualStyleBackColor = true;
        this.snoozeButton.Click += new System.EventHandler(this.SnoozeButton_Click);

        // ReminderForm
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(280, 120);
        this.Controls.Add(this.snoozeButton);
        this.Controls.Add(this.closeButton);
        this.Controls.Add(this.messageLabel);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "ReminderForm";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        this.Text = "站立提醒";
        this.TopMost = true;
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
