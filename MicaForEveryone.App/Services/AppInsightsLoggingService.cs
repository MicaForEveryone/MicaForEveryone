using MicaForEveryone.CoreUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MicaForEveryone.App.Services;

public partial class AppInsightsLoggingService : ILoggingService
{
    private string InstrumentationKey { get; }
    private readonly HttpClient _client;
    private List<(Exception Exception, DateTime Date)> _exceptions = new();
    private AppInsightsTags Tags { get; }
    private readonly ISettingsService _settingsService;

    public AppInsightsLoggingService(string connectionString, AppInsightsTags tags, ISettingsService settingsService)
    {
        _settingsService = settingsService;
        Span<Range> splitKeyValue = stackalloc Range[2];
        Uri? ingestionEndpoint = null;
        foreach (var keyValueStrings in MemoryExtensions.Split(connectionString, ';'))
        {
            ReadOnlySpan<char> slicedString = connectionString[keyValueStrings];
            MemoryExtensions.Split(slicedString, splitKeyValue, '=');
            ReadOnlySpan<char> key = slicedString[splitKeyValue[0]];
            ReadOnlySpan<char> value = slicedString[splitKeyValue[1]];

            if (key.SequenceEqual("InstrumentationKey"))
            {
                InstrumentationKey = value.ToString();
            }
            else if (key.SequenceEqual("IngestionEndpoint"))
            {
                ingestionEndpoint = new Uri(value.ToString());
            }
        }

        if (InstrumentationKey is null)
            throw new ArgumentException("The specified connection string does not contain an instrumentation key.");

        if (ingestionEndpoint is null)
        {
            ingestionEndpoint = new Uri("https://dc.services.visualstudio.com");
        }

        _client = new HttpClient() { BaseAddress = ingestionEndpoint };

        Tags = tags;
    }

    public void LogException(Exception exception)
    {
        _exceptions.Add((exception, DateTime.UtcNow));
    }

    public async Task FlushAsync()
    {
        List<AppInsightsRoot> roots = new();

        foreach (var exception in _exceptions)
        {
            List<AppInsightsExceptionInfo> exceptionInfos = new();
            int priorExceptionId = 0;
            Exception? currentException = exception.Exception;
            while (currentException is not null)
            {
                StackTrace stackTrace = new(currentException, true);
                var frames = stackTrace.GetFrames();
                Tuple<List<AppInsightsExceptionStackFrame>, bool> sanitizedTuple = SanitizeStackFrame(frames, GetStackFrame, GetStackFrameLength);
                int hashCode = currentException.GetHashCode();
                exceptionInfos.Add(
                    new AppInsightsExceptionInfo()
                    {
                        Id = hashCode,
                        HasFullStack = sanitizedTuple.Item2,
                        Message = currentException.Message,
                        OuterId = priorExceptionId,
                        ParsedStack = sanitizedTuple.Item1,
                        TypeName = currentException.GetType().FullName!
                    });
                priorExceptionId = hashCode;
                currentException = currentException.InnerException;
            }

            AppInsightsRoot root = new()
            {
                Name = "AppExceptions",
                Tags = Tags,
                Time = DateTime.UtcNow,
                InstrumentationKey = InstrumentationKey,
                Data = new AppInsightsExceptionData()
                {
                    BaseData = new AppInsightsExceptionBaseData()
                    {
                        Exceptions = exceptionInfos,
                    },
                    Version = 2
                }
            };

            roots.Add(root);
        }

        StringBuilder contentBuilder = new();
        for (int i = 0; i < roots.Count; i++)
        {
            contentBuilder.Append(JsonSerializer.Serialize(roots[i], AppInsightsSerializerContext.Default.AppInsightsRoot));
            if (i < roots.Count - 1)
            {
                contentBuilder.Append("\n");
            }
        }

        StringContent content = new(contentBuilder.ToString(), Encoding.UTF8, "application/x-json-stream");

        try
        {
            await _client.PostAsync("/v2/track", content).ConfigureAwait(false);
            roots.Clear();
        }
        catch { }
    }

    /// <summary>
    /// Converts a System.Diagnostics.StackFrame to a Microsoft.ApplicationInsights.Extensibility.Implementation.TelemetryTypes.StackFrame.
    /// </summary>
    internal static AppInsightsExceptionStackFrame GetStackFrame(StackFrame stackFrame, int frameId)
    {
        var convertedStackFrame = new AppInsightsExceptionStackFrame
        {
            Level = frameId,
        };

        var methodInfo = DiagnosticMethodInfo.Create(stackFrame);
        string fullName;
        string assemblyName;

        if (methodInfo == null)
        {
            fullName = "unknown";
            assemblyName = "unknown";
        }
        else
        {
            assemblyName = methodInfo.DeclaringAssemblyName;
            if (methodInfo.DeclaringTypeName != null)
            {
                fullName = methodInfo.DeclaringTypeName + "." + methodInfo.Name;
            }
            else
            {
                fullName = methodInfo.Name;
            }
        }

        convertedStackFrame.Method = fullName;
        convertedStackFrame.Assembly = assemblyName;
        convertedStackFrame.FileName = stackFrame.GetFileName();

        // 0 means it is unavailable
        int line = stackFrame.GetFileLineNumber();
        // The endpoint cannot ingest line number below -1000000 or above 1000000
        if (line != 0 && line > -1000000 && line < 1000000)
        {
            convertedStackFrame.Line = line;
        }

        return convertedStackFrame;
    }


    /// <summary>
    /// Gets the stack frame length for only the strings in the stack frame.
    /// </summary>
    internal static int GetStackFrameLength(AppInsightsExceptionStackFrame stackFrame)
    {
        var stackFrameLength = (stackFrame.Method == null ? 0 : stackFrame.Method.Length)
                               + (stackFrame.Assembly == null ? 0 : stackFrame.Assembly.Length)
                               + (stackFrame.FileName == null ? 0 : stackFrame.FileName.Length);
        return stackFrameLength;
    }


    /// <summary>
    /// Sanitizing stack to 32k while selecting the initial and end stack trace.
    /// </summary>
    private static Tuple<List<TOutput>, bool> SanitizeStackFrame<TInput, TOutput>(
        TInput[] inputList,
        Func<TInput, int, TOutput> converter,
        Func<TOutput, int> lengthGetter)
    {
        List<TOutput> orderedStackTrace = new List<TOutput>();
        bool hasFullStack = true;
        if (inputList != null && inputList.Length > 0)
        {
            int currentParsedStackLength = 0;
            for (int level = 0; level < inputList.Length; level++)
            {
                // Skip middle part of the stack
                int current = (level % 2 == 0) ? (inputList.Length - 1 - (level / 2)) : (level / 2);

                TOutput convertedStackFrame = converter(inputList[current], current);
                currentParsedStackLength += lengthGetter(convertedStackFrame);

                if (currentParsedStackLength > 32768)
                {
                    hasFullStack = false;
                    break;
                }

                orderedStackTrace.Insert(orderedStackTrace.Count / 2, convertedStackFrame);
            }
        }

        return new Tuple<List<TOutput>, bool>(orderedStackTrace, hasFullStack);
    }

    public class AppInsightsRoot
    {
        public required string Name { get; set; }

        public required DateTime Time { get; set; }

        [JsonPropertyName("iKey")]
        public required string InstrumentationKey { get; set; }

        public required AppInsightsTags Tags { get; set; }

        public required AppInsightsData Data { get; set; }
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "baseType", IgnoreUnrecognizedTypeDiscriminators = true)]
    [JsonDerivedType(typeof(AppInsightsExceptionData), "ExceptionData")]
    public abstract class AppInsightsData
    {
        [JsonPropertyName("ver")]
        public required int Version { get; set; }
    }

    public class AppInsightsExceptionData : AppInsightsData
    {
        public required AppInsightsExceptionBaseData BaseData { get; set; }
    }

    public class AppInsightsExceptionBaseData
    {
        public required List<AppInsightsExceptionInfo> Exceptions { get; set; }
    }

    public class AppInsightsExceptionInfo
    {
        public required int Id { get; set; }

        public int OuterId { get; set; } = 0;

        public required string TypeName { get; set; }

        public required string Message { get; set; }

        public required bool HasFullStack { get; set; }

        public required List<AppInsightsExceptionStackFrame> ParsedStack { get; set; }
    }

    public class AppInsightsExceptionStackFrame
    {
        public required int Level { get; set; }

        public string? FileName { get; set; }

        public string? Method { get; set; }

        public string? Assembly { get; set; }

        public int? Line { get; set; }
    }

    public class AppInsightsTags
    {
        [JsonPropertyName("ai.cloud.roleInstance")]
        public required string RoleInstance { get; set; }

        [JsonPropertyName("ai.internal.sdkVersion")]
        public string SdkVersion { get; set; } = "dotnetc:2.23.0-29";

        [JsonPropertyName("ai.application.ver")]
        public string? ApplicationVersion { get; set; }

        [JsonPropertyName("ai.device.osVersion")]
        public string? OSVersion { get; set; }

        [JsonPropertyName("ai.session.id")]
        public string? SessionId { get; set; }

        [JsonPropertyName("ai.user.id")]
        public string? UserId { get; set; }
    }

    [JsonSerializable(typeof(AppInsightsRoot), GenerationMode = JsonSourceGenerationMode.Metadata)]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public partial class AppInsightsSerializerContext : JsonSerializerContext;
}