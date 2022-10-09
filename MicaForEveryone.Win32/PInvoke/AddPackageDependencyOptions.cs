namespace MicaForEveryone.Win32.PInvoke
{
    public enum AddPackageDependencyOptions
    {
        /// <summary>No options are applied.</summary>
        AddPackageDependencyOptions_None,

        /// <summary>
        /// If multiple packages are present in the package graph with the same rank as the call to <c>AddPackageDependency</c>, the
        /// resolved package is added before others of the same rank.
        /// </summary>
        AddPackageDependencyOptions_PrependIfRankCollision,
    }
}