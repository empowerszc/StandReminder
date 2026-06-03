// StandReminder/SettingsForm.Designer.cs
namespace StandReminder;

partial class SettingsForm
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Label intervalLabel = null!;
    private System.Windows.Forms.NumericUpDown intervalNumeric = null!;
    private System.Windows.Forms.Label durationLabel = null!;
    private System.Windows.Forms.NumericUpDown durationNumeric = null!;
    private System.Windows.Forms.CheckBox autostartCheckBox = null!;
    private System.Windows.Forms.Button saveButton = null!;
    private System.Windows.Forms.Button cancelButton = null!;

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
        this.intervalLabel = new System.Windows.Forms.Label();
        this.intervalNumeric = new System.Windows.Forms.NumericUpDown();
        this.durationLabel = new System.Windows.Forms.Label();
        this.durationNumeric = new System.Windows.Forms.NumericUpDown();
        this.autostartCheckBox = new System.Windows.Forms.CheckBox();
        this.saveButton = new System.Windows.Forms.Button();
        this.cancelButton = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.intervalNumeric)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.durationNumeric)).BeginInit();
        this.SuspendLayout();

        // intervalLabel
        this.intervalLabel.AutoSize = true;
        this.intervalLabel.Location = new System.Drawing.Point(15, 20);
        this.intervalLabel.Name = "intervalLabel";
        this.intervalLabel.Size = new System.Drawing.Size(80, 15);
        this.intervalLabel.Text = "提醒间隔(分钟):";

        // intervalNumeric
        this.intervalNumeric.Location = new System.Drawing.Point(120, 18);
        this.intervalNumeric.Maximum = new decimal(new int[] { 240, 0, 0, 0 });
        this.intervalNumeric.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
        this.intervalNumeric.Name = "intervalNumeric";
        this.intervalNumeric.Size = new System.Drawing.Size(80, 23);
        this.intervalNumeric.Value = new decimal(new int[] { 60, 0, 0, 0 });

        // durationLabel
        this.durationLabel.AutoSize = true;
        this.durationLabel.Location = new System.Drawing.Point(15, 55);
        this.durationLabel.Name = "durationLabel";
        this.durationLabel.Size = new System.Drawing.Size(80, 15);
        this.durationLabel.Text = "站立时长(分钟):";

        // durationNumeric
        this.durationNumeric.Location = new System.Drawing.Point(120, 53);
        this.durationNumeric.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
        this.durationNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        this.durationNumeric.Name = "durationNumeric";
        this.durationNumeric.Size = new System.Drawing.Size(80, 23);
        this.durationNumeric.Value = new decimal(new int[] { 2, 0, 0, 0 });

        // autostartCheckBox
        this.autostartCheckBox.AutoSize = true;
        this.autostartCheckBox.Location = new System.Drawing.Point(18, 90);
        this.autostartCheckBox.Name = "autostartCheckBox";
        this.autostartCheckBox.Size = new System.Drawing.Size(75, 19);
        this.autostartCheckBox.Text = "开机自启";
        this.autostartCheckBox.UseVisualStyleBackColor = true;

        // saveButton
        this.saveButton.Location = new System.Drawing.Point(60, 125);
        this.saveButton.Name = "saveButton";
        this.saveButton.Size = new System.Drawing.Size(75, 28);
        this.saveButton.Text = "保存";
        this.saveButton.UseVisualStyleBackColor = true;
        this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);

        // cancelButton
        this.cancelButton.Location = new System.Drawing.Point(150, 125);
        this.cancelButton.Name = "cancelButton";
        this.cancelButton.Size = new System.Drawing.Size(75, 28);
        this.cancelButton.Text = "取消";
        this.cancelButton.UseVisualStyleBackColor = true;
        this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);

        // SettingsForm
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(280, 170);
        this.Controls.Add(this.cancelButton);
        this.Controls.Add(this.saveButton);
        this.Controls.Add(this.autostartCheckBox);
        this.Controls.Add(this.durationNumeric);
        this.Controls.Add(this.durationLabel);
        this.Controls.Add(this.intervalNumeric);
        this.Controls.Add(this.intervalLabel);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "SettingsForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "设置";
        ((System.ComponentModel.ISupportInitialize)(this.intervalNumeric)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.durationNumeric)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
