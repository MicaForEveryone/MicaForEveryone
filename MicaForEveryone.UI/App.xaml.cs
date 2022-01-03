using System;

namespace MicaForEveryone.UI
{
    public sealed partial class App
    {
        public App()
        {
            Initialize();
        }

        public IServiceProvider Container { get; set; }
    }
}
