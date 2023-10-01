using System.Text.Json.Serialization;

namespace MicaForEveryone.Models;

[JsonSerializable(typeof(SettingsModel))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow, IgnoreReadOnlyProperties = true, UseStringEnumConverter = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault, WriteIndented = true)]
public partial class MFESerializationContext : JsonSerializerContext
{
}