using System;
using System.Collections.Generic;
using System.Text;
using MicaForEveryone.Config.Parser;
using MicaForEveryone.Config.Reflection;

namespace MicaForEveryone.Config.Primitives
{
    public class XclStringType : XclType
    {
        public static XclStringType Instance { get; } = new();

        private XclStringType() : base("string")
        {
        }
    }
}
