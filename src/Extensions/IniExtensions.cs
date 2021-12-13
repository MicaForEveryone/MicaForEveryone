using System;
using IniParser.Model;
using MicaForEveryone.Rules;

namespace MicaForEveryone.Extensions
{
    public static class IniExtensions
    {
        public static TEnum ValueOrDefault<TEnum>(this KeyDataCollection data, string key, TEnum defaultValue = default) where TEnum : struct
        {
            return data.ContainsKey(key) ? Enum.Parse<TEnum>(data[key]) : defaultValue;
        }

        public static bool BoolOrDefault(this KeyDataCollection data, string key, bool defaultValue = false)
        {
            return data.ContainsKey(key) ? bool.Parse(data[key]) : defaultValue;
        }

        private static IRule GetRuleBySectionName(this SectionData data)
        {
            return data.SectionName.ToLower() switch
            {
                "global" => new GlobalRule(),
                "process" => new ProcessRule(data.Keys["Name"]),
                "classname" => new ClassRule(data.Keys["Name"]),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(data.SectionName),
                    data.SectionName,
                    "not a rule type")
            };
        }

        public static void ParseRule(this KeyDataCollection data, ref IRule rule)
        {
            rule.TitlebarColor = data.ValueOrDefault<TitlebarColorMode>("TitleBarColor");
            rule.MicaPreference = data.ValueOrDefault("MicaPreference", MicaPreference.PreferEnabled);
            rule.ExtendFrameIntoClientArea = data.BoolOrDefault("ExtendFrameIntoClientArea");
        }

        public static IRule ParseRule(this SectionData data)
        {
            var result = data.GetRuleBySectionName();
            data.Keys.ParseRule(ref result);
            return result;
        }
    }
}
