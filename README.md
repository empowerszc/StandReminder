# 站立提醒 (Stand Reminder)

一个轻量级 Windows 桌面托盘应用，定时提醒你站起来活动身体。

## 功能

- ⏰ 可配置提醒间隔（默认 60 分钟）
- 🧘 可配置站立时长（默认 2 分钟）
- 🔇 低调弹窗，位于屏幕右下角，不抢焦点，适合共享屏幕场景
- ⏸️ 支持"稍后提醒"（默认 5 分钟）
- 💤 电脑睡眠/息屏时自动暂停计时，唤醒后重新计时
- 🚀 开机自启动选项
- 📦 单文件可执行，无需安装 .NET 运行环境

## 截图

（待添加）

## 下载

前往 [Releases](https://github.com/YOUR_USERNAME/StandReminder/releases) 页面下载最新版本。

## 使用方法

1. 下载 `StandReminder.exe`
2. 双击运行，程序会最小化到系统托盘
3. 右键点击托盘图标，选择"设置"调整参数

## 托盘菜单

| 选项 | 说明 |
|------|------|
| 设置 | 调整提醒间隔、站立时长、开机自启 |
| 立即提醒 | 立即弹出提醒弹窗 |
| 退出 | 关闭程序 |

## 编译

需要 [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
# 编译
dotnet build StandReminder/StandReminder.csproj

# 运行测试
dotnet test StandReminder.Tests/StandReminder.Tests.csproj

# 发布单文件可执行程序
dotnet publish StandReminder/StandReminder.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

## 许可证

MIT
