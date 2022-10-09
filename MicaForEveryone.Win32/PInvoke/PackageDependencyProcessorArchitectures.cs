using System;

namespace MicaForEveryone.Win32.PInvoke
{
    [Flags]
    public enum PackageDependencyProcessorArchitectures
    {
        /// <summary>No processor architecture is specified.</summary>
        PackageDependencyProcessorArchitectures_None = 0x0000,

        /// <summary>Specifies the neutral architecture.</summary>
        PackageDependencyProcessorArchitectures_Neutral = 0x0001,

        /// <summary>Specifies the x86 architecture.</summary>
        PackageDependencyProcessorArchitectures_X86 = 0x0002,

        /// <summary>Specifies the x64 architecture.</summary>
        PackageDependencyProcessorArchitectures_X64 = 0x0004,

        /// <summary>Specifies the ARM architecture.</summary>
        PackageDependencyProcessorArchitectures_Arm = 0x0008,

        /// <summary>Specifies the ARM64 architecture.</summary>
        PackageDependencyProcessorArchitectures_Arm64 = 0x0010,

        /// <summary>Specifies the x86/A64 architecture.</summary>
        PackageDependencyProcessorArchitectures_X86A64 = 0x0020,
    }
}