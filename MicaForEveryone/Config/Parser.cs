using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MicaForEveryone.Config
{
    public class Parser
    {
        private enum SectionType
        {
            GlobalRule,
            ProcessRule,
            ClassRule,
            Override,
        }

        private enum KeyName
        {
            TitleBarColor,
            BackdropPreference,
            ExtendFrameToClientArea,
        }

    }
}
