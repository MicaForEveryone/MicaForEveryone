using System.Runtime.InteropServices;

namespace MicaForEveryone.PInvoke;

public static partial class Modules
{
    #region Type Declarations
    public readonly unsafe struct HINSTANCE
    {
        public readonly void* Value;

        public HINSTANCE(void* value)
        {
            Value = value;
        }
    }
    #endregion

    #region Methods
    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial HINSTANCE GetModuleHandleW(string? moduleName);
    #endregion
}
