using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

using MicaForEveryone.Views;

namespace MicaForEveryone.Interfaces
{
    internal interface ITrayIconViewModel : UI.ViewModels.ITrayIconViewModel
    {
        Task InitializeAsync(MainWindow sender);

        bool TrayIconVisible { get; set; }
    }
}
