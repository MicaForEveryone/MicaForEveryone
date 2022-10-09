using System;

namespace MicaForEveryone.Win32.PInvoke
{
    [Flags]
    public enum CreatePackageDependencyOptions
    {
        /// <summary>No options are applied.</summary>
        CreatePackageDependencyOptions_None = 0,

        /// <summary>
        /// Disables dependency resolution when pinning a package dependency. This is useful for installers running as user contexts
        /// other than the target user (for example, installers running as LocalSystem).
        /// </summary>
        CreatePackageDependencyOptions_DoNotVerifyDependencyResolution = 1,

        /// <summary>
        /// Defines the package dependency for the system, accessible to all users (by default, the package dependency is defined for a
        /// specific user). This option requires the caller has administrative privileges.
        /// </summary>
        CreatePackageDependencyOptions_ScopeIsSystem = 2,
    }
}