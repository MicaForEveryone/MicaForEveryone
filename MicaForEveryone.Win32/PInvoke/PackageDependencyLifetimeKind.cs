namespace MicaForEveryone.Win32.PInvoke
{
    public enum PackageDependencyLifetimeKind
    {
        /// <summary>
        /// The current process is the lifetime artifact. The package dependency is implicitly deleted when the process terminates.
        /// </summary>
        PackageDependencyLifetimeKind_Process,

        /// <summary>
        /// The lifetime artifact is an absolute filename or path. The package dependency is implicitly deleted when this is deleted.
        /// </summary>
        PackageDependencyLifetimeKind_FilePath,

        /// <summary>
        /// <para>The lifetime artifact is a registry key in the format</para>
        /// <para>root</para>
        /// <para>\</para>
        /// <para>subkey</para>
        /// <para>, where</para>
        /// <para>root</para>
        /// <para>
        /// is one of the following: HKEY_LOCAL_MACHINE, HKEY_CURRENT_USER, HKEY_CLASSES_ROOT, or HKEY_USERS. The package dependency is
        /// implicitly deleted when this is deleted.
        /// </para>
        /// </summary>
        PackageDependencyLifetimeKind_RegistryKey,
    }
}