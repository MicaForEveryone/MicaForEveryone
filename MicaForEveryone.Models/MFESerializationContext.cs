using System.Text.Json.Serialization;

namespace MicaForEveryone.Models;

[JsonSerializable(typeof(SettingsModel))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, UseStringEnumConverter = true)]
public partial class MFESerializationContext : JsonSerializerContext
{
}