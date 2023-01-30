using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

using MicaForEveryone.Views;

namespace MicaForEveryone.Interfaces
{
    internal interface ITrayIconViewModel : Core.Ui.ViewModels.ITrayIconViewModel
    {
        void Initialize(MainWindow sender);

        bool TrayIconVisible { get; set; }
    }
}
