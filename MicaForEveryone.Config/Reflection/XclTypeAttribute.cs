using System;

namespace MicaForEveryone.Config.Reflection
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Class)]
    public class XclTypeAttribute : Attribute
    {
        public string TypeName { get; set; }
    }
}
