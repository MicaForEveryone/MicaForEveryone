using System;

namespace MicaForEveryone.Win32.PInvoke
{
    [Flags]
    public enum MB_RESULT
    {
        /// <summary>The Abort button was selected.</summary>
        IDABORT = 3,

        /// <summary>The Cancel button was selected.</summary>
        IDCANCEL = 2,

        /// <summary>The Continue button was selected.</summary>
        IDCONTINUE = 11,

        /// <summary>The Ignore button was selected.</summary>
        IDIGNORE = 5,

        /// <summary>The No button was selected.</summary>
        IDNO = 7,

        /// <summary>The OK button was selected.</summary>
        IDOK = 1,

        /// <summary>The Retry button was selected.</summary>
        IDRETRY = 4,

        /// <summary>The Try Again button was selected.</summary>
        IDTRYAGAIN = 10,

        /// <summary>The Yes button was selected.</summary>
        IDYES = 6,
    }
}
