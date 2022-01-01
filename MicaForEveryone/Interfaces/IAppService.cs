using System;

using MicaForEveryone.Win32;

namespace MicaForEveryone.Interfaces
{
    public interface IAppService
    {
        void Run(NativeWindow window);

        void Exit();
    }
}
