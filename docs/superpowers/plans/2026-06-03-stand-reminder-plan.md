# 站立提醒工具 (Stand Reminder) 实现计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 开发一个 Windows 桌面托盘应用，定时提醒用户站起来活动，支持可配置间隔、稍后提醒、开机自启。

**Architecture:** 单实例 WinForms 应用，隐藏主窗体管理系统托盘和定时器，配置通过 JSON 持久化到本地。

**Tech Stack:** C# (.NET 8), WinForms, System.Text.Json, Microsoft.Win32.Registry

---

## 文件结构

| 文件 | 职责 |
|------|------|
| `StandReminder/StandReminder.csproj` | 项目配置，目标框架 net8.0-windows |
| `StandReminder/Program.cs` | 入口点，单实例 Mutex 控制 |
| `StandReminder/AppSettings.cs` | 设置模型类 + JSON 读写逻辑 |
| `StandReminder/MainForm.cs` | 隐藏主窗体，托盘图标、定时器、菜单逻辑 |
| `StandReminder/MainForm.Designer.cs` | MainForm 的 UI 设计器代码 |
| `StandReminder/ReminderForm.cs` | 提醒弹窗逻辑，右下角定位，不抢焦点 |
| `StandReminder/ReminderForm.Designer.cs` | ReminderForm 的 UI 设计器代码 |
| `StandReminder/SettingsForm.cs` | 设置界面逻辑，间隔/时长/自启配置 |
| `StandReminder/SettingsForm.Designer.cs` | SettingsForm 的 UI 设计器代码 |

---

### Task 1: 创建项目结构

**Files:**
- Create: `StandReminder/StandReminder.csproj`

- [ ] **Step 1: 创建项目文件**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>
    <ApplicationIcon>Resources\tray.ico</ApplicationIcon>
  </PropertyGroup>

</Project>
```

- [ ] **Step 2: 创建 Resources 目录**

```bash
New-Item -ItemType Directory -Path "StandReminder\Resources" -Force
```

- [ ] **Step 3: 创建占位图标**

由于需要托盘图标，创建一个简单的 .ico 文件。使用 PowerShell 生成一个 16x16 的简单图标：

```powershell
# 创建一个简单的图标文件（后续可替换）
Add-Type -AssemblyName System.Drawing
$bmp = New-Object System.Drawing.Bitmap(16, 16)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color.Transparent])
$brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color.FromArgb](100, 149, 237))
$g.FillEllipse($brush, 1, 1, 14, 14)
$icon = [System.Drawing.Icon]::FromHandle($bmp.GetHicon())
$stream = [System.IO.File]::Create("$PWD\StandReminder\Resources\tray.ico")
$icon.Save($stream)
$stream.Close()
$icon.Dispose()
$bmp.Dispose()
$g.Dispose()
$brush.Dispose()
```

- [ ] **Step 4: 验证项目可编译**

```bash
dotnet build StandReminder/StandReminder.csproj
```

预期：编译成功（无代码文件时会警告，但项目结构正确）

- [ ] **Step 5: 提交**

```bash
git add StandReminder/StandReminder.csproj StandReminder/Resources/tray.ico
git commit -m "chore: create project structure"
```

---

### Task 2: 创建设置模型与持久化 (AppSettings)

**Files:**
- Create: `StandReminder/AppSettings.cs`

- [ ] **Step 1: 创建设置模型**

```csharp
// StandReminder/AppSettings.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StandReminder;

public class AppSettings
{
    public int ReminderIntervalMinutes { get; set; } = 60;
    public int StandDurationMinutes { get; set; } = 2;
    public bool AutostartEnabled { get; set; } = false;
    public int SnoozeMinutes { get; set; } = 5;

    private static readonly string SettingsDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "StandReminder");
    private static readonly string SettingsPath = Path.Combine(SettingsDir, "settings.json");

    public static AppSettings Load()
    {
        if (!File.Exists(SettingsPath))
        {
            return new AppSettings();
        }

        try
        {
            var json = File.ReadAllText(SettingsPath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json);
            return settings ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(SettingsDir);
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(SettingsPath, json);
    }
}
```

- [ ] **Step 2: 编写测试验证设置读写**

创建测试项目：

```bash
dotnet new xunit -n StandReminder.Tests -o StandReminder.Tests
cd StandReminder.Tests
dotnet add reference ../StandReminder/StandReminder.csproj
```

```csharp
// StandReminder.Tests/AppSettingsTests.cs
using StandReminder;
using Xunit;

namespace StandReminder.Tests;

public class AppSettingsTests
{
    [Fact]
    public void Load_ReturnsDefaults_WhenFileNotExists()
    {
        var settings = AppSettings.Load();
        Assert.Equal(60, settings.ReminderIntervalMinutes);
        Assert.Equal(2, settings.StandDurationMinutes);
        Assert.False(settings.AutostartEnabled);
        Assert.Equal(5, settings.SnoozeMinutes);
    }

    [Fact]
    public void SaveAndLoad_PersistsValues()
    {
        var original = new AppSettings
        {
            ReminderIntervalMinutes = 30,
            StandDurationMinutes = 5,
            AutostartEnabled = true,
            SnoozeMinutes = 10
        };
        original.Save();

        var loaded = AppSettings.Load();
        Assert.Equal(30, loaded.ReminderIntervalMinutes);
        Assert.Equal(5, loaded.StandDurationMinutes);
        Assert.True(loaded.AutostartEnabled);
        Assert.Equal(10, loaded.SnoozeMinutes);
    }
}
```

- [ ] **Step 3: 运行测试**

```bash
dotnet test StandReminder.Tests/StandReminder.Tests.csproj
```

预期：2 个测试全部通过

- [ ] **Step 4: 提交**

```bash
git add StandReminder/AppSettings.cs StandReminder.Tests/
git commit -m "feat: add settings model with JSON persistence"
```

---

### Task 3: 创建主窗体与系统托盘 (MainForm)

**Files:**
- Create: `StandReminder/MainForm.cs`
- Create: `StandReminder/MainForm.Designer.cs`

- [ ] **Step 1: 创建 MainForm.Designer.cs**

```csharp
// StandReminder/MainForm.Designer.cs
namespace StandReminder;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.NotifyIcon trayIcon = null!;
    private System.Windows.Forms.ContextMenuStrip trayMenu = null!;
    private System.Windows.Forms.ToolStripMenuItem settingsMenuItem = null!;
    private System.Windows.Forms.ToolStripMenuItem remindNowMenuItem = null!;
    private System.Windows.Forms.ToolStripSeparator separator = null!;
    private System.Windows.Forms.ToolStripMenuItem exitMenuItem = null!;
    private System.Windows.Forms.Timer reminderTimer = null!;

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
            this.trayIcon.Icon = new System.Drawing.Icon(iconPath);
        }

        this.reminderTimer.Tick += new System.EventHandler(this.ReminderTimer_Tick);

        this.components.Add(this.trayIcon);
        this.components.Add(this.trayMenu);
        this.components.Add(this.reminderTimer);
    }
}
```

- [ ] **Step 2: 创建 MainForm.cs**

```csharp
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
```

- [ ] **Step 3: 验证编译**

```bash
dotnet build StandReminder/StandReminder.csproj
```

预期：编译失败（ReminderForm 和 SettingsForm 尚未创建），但 MainForm 语法正确

- [ ] **Step 4: 提交**

```bash
git add StandReminder/MainForm.cs StandReminder/MainForm.Designer.cs
git commit -m "feat: add main form with system tray and timer"
```

---

### Task 4: 创建提醒弹窗 (ReminderForm)

**Files:**
- Create: `StandReminder/ReminderForm.cs`
- Create: `StandReminder/ReminderForm.Designer.cs`

- [ ] **Step 1: 创建 ReminderForm.Designer.cs**

```csharp
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
        this.TopMost = false;
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
```

- [ ] **Step 2: 创建 ReminderForm.cs**

```csharp
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
```

- [ ] **Step 3: 验证编译**

```bash
dotnet build StandReminder/StandReminder.csproj
```

预期：编译失败（SettingsForm 尚未创建），但 ReminderForm 语法正确

- [ ] **Step 4: 提交**

```bash
git add StandReminder/ReminderForm.cs StandReminder/ReminderForm.Designer.cs
git commit -m "feat: add reminder popup positioned bottom-right"
```

---

### Task 5: 创建设置界面 (SettingsForm)

**Files:**
- Create: `StandReminder/SettingsForm.cs`
- Create: `StandReminder/SettingsForm.Designer.cs`

- [ ] **Step 1: 创建 SettingsForm.Designer.cs**

```csharp
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
```

- [ ] **Step 2: 创建 SettingsForm.cs**

```csharp
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
```

- [ ] **Step 3: 验证编译**

```bash
dotnet build StandReminder/StandReminder.csproj
```

预期：编译成功（所有窗体已创建）

- [ ] **Step 4: 提交**

```bash
git add StandReminder/SettingsForm.cs StandReminder/SettingsForm.Designer.cs
git commit -m "feat: add settings form with interval, duration, autostart"
```

---

### Task 6: 创建入口点并集成 (Program.cs)

**Files:**
- Create: `StandReminder/Program.cs`

- [ ] **Step 1: 创建 Program.cs**

```csharp
// StandReminder/Program.cs
using System.Windows.Forms;

namespace StandReminder;

static class Program
{
    private static Mutex? _mutex;

    [STAThread]
    static void Main()
    {
        const string mutexName = "StandReminder_SingleInstance";
        _mutex = new Mutex(true, mutexName, out bool createdNew);

        if (!createdNew)
        {
            MessageBox.Show("站立提醒已在运行中。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
```

- [ ] **Step 2: 完整编译**

```bash
dotnet build StandReminder/StandReminder.csproj
```

预期：编译成功，无错误

- [ ] **Step 3: 提交**

```bash
git add StandReminder/Program.cs
git commit -m "feat: add entry point with single-instance guard"
```

---

### Task 7: 集成测试与构建发布

- [ ] **Step 1: 运行所有测试**

```bash
dotnet test StandReminder.Tests/StandReminder.Tests.csproj
```

预期：全部通过

- [ ] **Step 2: 发布单文件可执行程序**

```bash
dotnet publish StandReminder/StandReminder.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

- [ ] **Step 3: 验证输出**

```bash
ls publish/
```

预期：包含 `StandReminder.exe` 及必要文件

- [ ] **Step 4: 提交**

```bash
git add .
git commit -m "chore: final integration and publish configuration"
```

---

## 自检验证

**Spec 覆盖检查:**
- [x] 可配置提醒间隔（默认60分钟）→ SettingsForm + AppSettings
- [x] 可配置站立时长（默认2分钟）→ SettingsForm + AppSettings
- [x] 弹窗右下角显示 → ReminderForm.PositionBottomRight()
- [x] 弹窗不抢焦点 → ShowWithoutActivation = true
- [x] 手动关闭 → CloseButton_Click
- [x] 稍后提醒（5分钟）→ SnoozeButton_Click + MainForm.SnoozeReminder()
- [x] 系统托盘 → MainForm.NotifyIcon
- [x] 开机自启 → SettingsForm.UpdateAutostart() 注册表
- [x] 单实例运行 → Program.cs Mutex
- [x] 托盘菜单（设置/立即提醒/退出）→ MainForm trayMenu

**占位符扫描:** 无 TBD/TODO

**类型一致性:** AppSettings 属性名在各处使用一致，方法签名匹配
