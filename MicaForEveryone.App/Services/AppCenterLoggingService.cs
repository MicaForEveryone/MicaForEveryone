using MicaForEveryone.CoreUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.System.Profile;

namespace MicaForEveryone.App.Services;

public sealed partial class AppCenterLoggingService : ILoggingService
{
    private readonly int _processId;
    private readonly string _processName;
    private readonly DateTime _appLaunchTimestamp;
    private readonly IVersionInfoService _versionInfoService;
    private readonly ISettingsService _settingsService;
    private readonly HttpClient _client;
    private List<LogInfo> _exceptions = new();

    public AppCenterLoggingService(string AppSecret, string InstallId, IVersionInfoService versionInfoService, ISettingsService settingsService)
    {
        ArgumentException.ThrowIfNullOrEmpty(AppSecret, nameof(AppSecret));
        ArgumentException.ThrowIfNullOrEmpty(InstallId, nameof(InstallId));

        Process proc = Process.GetCurrentProcess();
        _processId = proc.Id;
        _processName = proc.ProcessName;
        _appLaunchTimestamp = proc.StartTime.ToLocalTime();

        _versionInfoService = versionInfoService;
        _client = new();
        _client.DefaultRequestHeaders.Add("app-secret", AppSecret);
        _client.DefaultRequestHeaders.Add("install-id", InstallId);

        _settingsService = settingsService;
    }

    public async Task FlushAsync()
    {
        if (_settingsService.Settings?.TelemetryEnabled != true)
            return;

        LogsRoot logs = new()
        {
            Logs = _exceptions
        };
        string serialized = JsonSerializer.Serialize(logs, AppCenterSerializerContext.Default.LogsRoot);
        await _client.PostAsync("https://in.appcenter.ms/logs?Api-Version=1.0.0", new StringContent(serialized, Encoding.UTF8, "application/json"));
        _exceptions.Clear();
    }

    public void LogException(Exception exception)
    {
        var version = _versionInfoService.GetVersion();
        _exceptions.Add(
            new LogInfo()
            {
                AppLaunchTimestamp = _appLaunchTimestamp,
                Fatal = true,
                Id = Guid.NewGuid(),
                ProcessName = _processName,
                Type = "managedError",
                Timestamp = DateTime.UtcNow,
                Device = new DeviceInfo()
                {
                    AppBuild = version,
                    AppVersion = version,
                    Locale = GetLocale(),
                    OsName = "WINDOWS",
                    OsVersion = GetOsVersion(),
                    SdkName = "appcenter.uwp",
                    SdkVersion = "5.0.7"
                },
                Exception = ConvertExceptionToInfo(exception)
            });
    }

    private static string GetOsVersion()
    {
        ulong num = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
        ulong num2 = (num & 0xFFFF000000000000uL) >> 48;
        ulong num3 = (num & 0xFFFF00000000L) >> 32;
        ulong num4 = (num & 0xFFFF0000u) >> 16;
        return $"{num2}.{num3}.{num4}";
    }

    private static unsafe string GetLocale()
    {
        char* locale = stackalloc char[530];
        if (GetLocaleInfoEx(null, 92u, locale, 530) > 0)
            return new string(locale);
        fixed (char* sysDefault = "!x-sys-default-locale")
        {
            if (GetLocaleInfoEx(sysDefault, 92u, locale, 530) > 0)
                return new string(locale);
        }
        return CultureInfo.CurrentCulture.Name;
    }

    [DllImport("kernel32", ExactSpelling = true)]
    private static unsafe extern int GetLocaleInfoEx(char* lpLocaleName, uint LCType, char* lpLCData, int cchData);

    private ExceptionInfo ConvertExceptionToInfo(Exception exception)
    {
        StackTrace trace = new StackTrace(exception);
        System.Diagnostics.StackFrame[] managedFrames = trace.GetFrames();
        List<StackFrame> frame = new(managedFrames.Length);
        foreach (var managedFrame in managedFrames)
        {
            DiagnosticMethodInfo? methodInfo = DiagnosticMethodInfo.Create(managedFrame);
            frame.Add(new StackFrame()
            {
                FileName = managedFrame.GetFileName(),
                LineNumber = managedFrame.GetFileLineNumber(),
                MethodName = methodInfo?.Name ?? null
            });
        }

        List<ExceptionInfo>? innerExceptions = null;
        if (exception.InnerException is Exception innerException)
        {
            innerExceptions = new(1);
            innerExceptions.Add(ConvertExceptionToInfo(innerException));
        }

        return new ExceptionInfo()
        {
            Type = exception.GetType().FullName!,
            Frame = frame,
            StackTrace = exception.StackTrace,
            InnerExceptions = innerExceptions,
            Message = exception.Message
        };
    }

    public class LogsRoot
    {
        public required List<LogInfo> Logs { get; set; }
    }

    public class LogInfo
    {
        public required string Type { get; set; }

        public required DateTime AppLaunchTimestamp { get; set; }

        public DateTime? Timestamp { get; set; }

        public string? UserId { get; set; }

        public required bool Fatal { get; set; }

        public required string ProcessName { get; set; }

        public required Guid Id { get; set; }

        public required DeviceInfo Device { get; set; }

        public required ExceptionInfo Exception { get; set; }
    }

    public class DeviceInfo
    {
        public required string AppVersion { get; set; }

        public required string AppBuild { get; set; }

        public required string SdkName { get; set; }

        public required string SdkVersion { get; set; }

        public required string OsName { get; set; }

        public required string OsVersion { get; set; }

        public string? Model { get; set; }

        public required string Locale { get; set; }

        public int? TimeZoneOffset { get; set; }
    }

    public class ExceptionInfo
    {
        public required string Type { get; set; }

        public string? Message { get; set; }

        public string? StackTrace { get; set; }

        public List<StackFrame>? Frame { get; set; }

        public List<ExceptionInfo>? InnerExceptions { get; set; }
    }

    public class StackFrame
    {
        public string? Address { get; set; }

        public string? Code { get; set; }

        public string? ClassName { get; set; }

        public string? MethodName { get; set; }

        public int? LineNumber { get; set; }

        public string? FileName { get; set; }
    }

    [JsonSerializable(typeof(LogsRoot), GenerationMode = JsonSourceGenerationMode.Metadata)]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public partial class AppCenterSerializerContext : JsonSerializerContext;
}
