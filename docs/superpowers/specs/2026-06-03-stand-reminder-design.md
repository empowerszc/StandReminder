# 站立提醒工具 - 设计规格文档

## 概述
一个轻量级 Windows 桌面应用程序，以可配置的间隔提醒用户站起来活动。运行在系统托盘中，UI 极简。

## 架构

### 技术栈
- **语言**: C# (.NET 8)
- **UI 框架**: Windows Forms (WinForms)
- **目标平台**: Windows x86/x64 (兼容 Intel i7)
- **输出**: 单文件可执行程序 (.exe)

### 项目结构
```
StandReminder/
├── StandReminder.csproj
├── Program.cs              # 入口点，单实例控制
├── MainForm.cs             # 隐藏主窗体，管理托盘和定时器
├── MainForm.Designer.cs
├── SettingsForm.cs         # 设置界面
├── SettingsForm.Designer.cs
├── ReminderForm.cs         # 低调提醒弹窗
├── ReminderForm.Designer.cs
├── AppSettings.cs          # 设置模型与持久化
└── Resources/
    └── tray.ico            # 系统托盘图标
```

## 组件说明

### 1. Program.cs
- 程序入口
- 通过 `Mutex` 实现单实例运行（防止重复启动）
- 初始化 `MainForm`

### 2. MainForm（主窗体）
- 隐藏窗体（`Visible = false`, `ShowInTaskbar = false`）
- **NotifyIcon**: 系统托盘图标及右键菜单
  - 菜单项：设置、立即提醒、退出
- **Timer**: 可配置间隔，到期触发 `ReminderForm`
- **开机自启管理**: 通过注册表实现

### 3. SettingsForm（设置界面）
- 配置项：
  - 提醒间隔（分钟，默认：60）
  - 站立时长（分钟，默认：2）
  - 开机自启（复选框）
- 保存/取消按钮
- 配置持久化到 `%LOCALAPPDATA%\StandReminder\settings.json`

### 4. ReminderForm（提醒弹窗）
- **位置**: 主屏幕右下方，靠近系统托盘区域
- **尺寸**: 约 300x150 像素
- **样式**: 浅色主题，无边框强调，不抢夺焦点
- **内容**: "站起来活动一下（X分钟）"
- **按钮**: [关闭] [稍后(5分钟)]
- **行为**: 
  - 显示时不激活窗口（`ShowWithoutActivation = true`）
  - 不强制置顶
  - 点击按钮后关闭，重置定时器或触发稍后提醒

### 5. AppSettings（配置管理）
```json
{
  "ReminderIntervalMinutes": 60,
  "StandDurationMinutes": 2,
  "AutostartEnabled": false,
  "SnoozeMinutes": 5
}
```
- 使用 `System.Text.Json` 序列化/反序列化
- 首次运行时自动创建默认配置

## 核心流程

1. **启动**
   - 从 JSON 加载配置
   - 隐藏主窗体
   - 显示系统托盘图标
   - 启动倒计时定时器

2. **提醒触发**
   - 定时器到期 → 创建 `ReminderForm`
   - 定位到屏幕右下角
   - 静默显示（不抢焦点）

3. **用户操作**
   - **关闭**: 关闭弹窗，以完整间隔重启定时器
   - **稍后提醒**: 关闭弹窗，以稍后间隔（5分钟）重启定时器

4. **托盘菜单**
   - **设置**: 打开 `SettingsForm`
   - **立即提醒**: 立即显示 `ReminderForm`
   - **退出**: 释放托盘图标，保存配置，退出程序

## 开机自启实现
- 注册表路径: `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`
- 值名称: `StandReminder`
- 值数据: 可执行文件完整路径

## 异常处理
- 配置文件缺失/损坏 → 创建默认配置
- 定时器线程安全 → 使用 `Invoke` 更新 UI
- 单实例控制 → 已运行时提示用户

## 构建配置
- 目标框架: `net8.0-windows`
- 输出类型: `WinExe`
- 发布: 单文件，自包含（可选）
- 平台: `x64`

## 依赖项
- .NET 8 SDK
- 无第三方包（使用内置 `System.Text.Json`、`Microsoft.Win32.Registry`）
