using System;
using System.Collections.Generic;
using System.Text;

namespace MicaForEveryone.Models
{
    public interface IFocusableWindow
    {
        event EventHandler GotFocus;
        event EventHandler LostFocus;
    }
}
